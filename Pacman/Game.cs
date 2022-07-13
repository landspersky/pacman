using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;

namespace Pacman
{
    public enum KeyPressed { none, left, up, right, down };

    public enum Direction {  left, up, right, down };

    abstract class Character
    {
        protected Map map;
        public int slowness; // period of movement in ticks
        public int x;
        public int y;
        public abstract void Move();

        protected Character(Map map, int x, int y)
        {
            this.map = map;
            this.x = x;
            this.y = y;
        }

        protected (int, int) TargetCoords(Direction direction, int from_x, int from_y)
        {
            // returns (x, y) of the field that is 'direction' of (from_x, from_y)
            int target_x = this.x;
            int target_y = this.y;
            switch (direction)
            {
                case Direction.left:
                    from_x--;
                    break;
                case Direction.up:
                    from_y--;
                    break;
                case Direction.right:
                    from_x++;
                    break;
                case Direction.down:
                    from_y++;
                    break;
                default:
                    break;
            }
            return (from_x, from_y);
        }

        protected (int, int) TargetCoords(Direction direction)
        {
            // return (x, y) of field 'direction' of character
            return TargetCoords(direction, x, y);
        }

        protected bool IsFreeSpace(Direction direction, int from_x, int from_y)
        {
            var coords = TargetCoords(direction, from_x, from_y);
            return map.IsFreeSpace(coords);
        }

        protected bool IsFreeSpace(Direction direction)
        {
            return IsFreeSpace(direction, x, y);
        }
    }
    abstract class Ghost : Character
    {
        public char id;
        public Pacman pacman;
        protected Ghost(Map map, int x, int y) : base(map, x, y)
        {
        }
        protected (int, int) NextOnShortest(int to_x, int to_y)
        {
            // returns the next location on the shortest path to (to_x, to_y)
            Queue<(int, int)> q = new Queue<(int, int)>();
            q.Enqueue((x, y));
            Dictionary<(int, int), (int, int)> firstParent = new Dictionary<(int, int), (int, int)>();

            //int original_x = x;
            //int original_y = y;
            (int, int) current = (0, 0);

            while (! firstParent.ContainsKey((to_x, to_y)))
            {
                current = q.Dequeue();
                //x = current.Item1;
                //y = current.Item2;
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    (int, int) neighbour = TargetCoords(d, current.Item1, current.Item2);
                    if (! firstParent.ContainsKey(neighbour) && map.IsFreeSpace(neighbour))
                    {
                        q.Enqueue(neighbour);
                        if (firstParent.ContainsKey(current))
                            { firstParent[neighbour] = firstParent[current]; }
                        else
                            { firstParent[neighbour] = neighbour; }
                    }
                }
            }
            //x = original_x;
            //y = original_y;
            return firstParent[(to_x, to_y)];
        }
    }

    class RedGhost : Ghost
    {
        public RedGhost(Map map, int x, int y) : base(map, x, y)
        {
            id = 'r';
            slowness = 4;
        }

        public override void Move()
        {
            (int, int) to = NextOnShortest(pacman.x, pacman.y);
            x = to.Item1;
            y = to.Item2;
        }
    }

    class PinkGhost : Ghost
    {
        public PinkGhost(Map map, int x, int y) : base(map, x, y)
        {
            id = 'p';
            slowness = 3;
        }

        public override void Move()
        {
            
        }
    }
    class Pacman : Character
    {
        public bool opened; // altering between two icons
        public Direction direction = Direction.left;

        public Pacman(Map map, int x, int y) : base(map, x, y)
        {
            slowness = 2;
        }

        public void Turn(KeyPressed key)
        {
            switch (key)
            {
                case KeyPressed.left:
                    if (IsFreeSpace(Direction.left))
                        { this.direction = Direction.left; }
                    break;
                case KeyPressed.up:
                    if (IsFreeSpace(Direction.up))
                        { this.direction = Direction.up; }
                    break;
                case KeyPressed.right:
                    if (IsFreeSpace(Direction.right))
                        { this.direction = Direction.right; }
                    break;
                case KeyPressed.down:
                    if (IsFreeSpace(Direction.down))
                        { this.direction = Direction.down; }
                    break;
                default:
                    break;
            }
        }

        public override void Move()
        {
            (int, int) new_coords = TargetCoords(direction);

            if (map.IsFreeSpace(new_coords))
            {
                map.MovePacman(x, y, new_coords.Item1, new_coords.Item2);
            }
        }
    }

    class StatusBar
    {
        public int coinsLeft = 0;
        public int livesLeft;
        public int score;
        public int width;
        public int height = 50;

        private Label lLives;
        private Label lScore;
        private Button bMenu;

        public StatusBar(Label lLives, Label lScore, Button bMenu)
        {
            this.lLives = lLives;
            this.lScore = lScore;
            this.bMenu = bMenu;
        }

        public void Draw(Map map)
        {
            int startx = map.startx;
            int starty = map.starty - height;
            int width = map.width * map.sx; // width is in pixels
            int midx = startx + width / 2;
            int midy = starty + height / 2;
            int padding = 10;

            // Lives
            lLives.Text = $"Lives: {livesLeft}";
            lLives.Left = startx + padding;
            lLives.Top = midy - lLives.Height / 2;

            // Score
            lScore.Text = $"{score}";
            lScore.Left = midx - lScore.Width / 2;
            lScore.Top = midy - lScore.Height / 2;

            // Menu
            bMenu.Left = startx + width - padding - bMenu.Width;
            bMenu.Top = midy - bMenu.Height / 2;

        }
    }

    public enum State { running, win, loss };

    class Map
    {
        private char[,] plan;
        public int width;
        public int height;
        public int startx;
        public int starty;

        public State state = State.running;

        Bitmap[] icons;
        public int sx;

        public Pacman pacman;
        public StatusBar statusbar;
        public List<Ghost> ghosts;
        // other atributes

        public Map(Form1 form, string mapPath, string iconsPath, StatusBar statusBar)
        {
            this.statusbar = statusBar;
            LoadIcons(iconsPath);
            LoadMap(mapPath);

            int padding = 10;
            form.MinimumSize = new Size(width * sx + 2 * padding,
                height * sx + 2 * (statusbar.height + padding) );
        }

        public bool IsFreeSpace((int, int) coords)
        {
            return plan[coords.Item1, coords.Item2] != 'X';
        }

        public void Draw(Graphics g, int windowWidth, int windowHeight)
        {
            pacman.opened = !pacman.opened;
            int midx = windowWidth / 2;
            int midy = windowHeight / 2;
            startx = midx - width * sx / 2;
            starty = midy - height * sx / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char c = plan[x, y];
                    int indexObrazku = 0;
                    if (c == 'P')
                    {
                        // the icons are in the same order as enum Direction
                        // there are twice as many (opened and closed) and they start on index 8
                        if (pacman.opened)
                            { indexObrazku++; }
                        indexObrazku += 8 + 2 * (int)pacman.direction;
                    }
                    else 
                        { indexObrazku = " X.$".IndexOf(c); }
                    g.DrawImage(icons[indexObrazku], x * sx + startx, y * sx + starty);
                }
            }

            foreach (Ghost gh in ghosts)
            {
                int indexObrazku = "rpbo".IndexOf(gh.id) + 4;
                g.DrawImage(icons[indexObrazku], gh.x * sx + startx, gh.y * sx + starty);
            }
        }

        public void LoadIcons(string path)
        {
            // loads icons stored in one file given by path into Bitmap icons
            // the icons have to be next to each other with no overlay or padding
            Bitmap bmp = new Bitmap(path);
            this.sx = bmp.Height;
            int count = bmp.Width / sx;
            icons = new Bitmap[count];
            for (int i = 0; i < count; i++)
            {
                Rectangle rect = new Rectangle(i * sx, 0, sx, sx);
                icons[i] = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.DontCare);
            }
        }

        public void LoadMap(string path)
        {
            ghosts = new List<Ghost>();
            StreamReader sr = new StreamReader(path);
            width = int.Parse(sr.ReadLine());
            height = int.Parse(sr.ReadLine());
            plan = new char[width, height];

            for (int y = 0; y < height; y++)
            {
                string line = sr.ReadLine();
                for (int x = 0; x < width; x++)
                {
                    char sign = line[x];
                    plan[x, y] = sign;

                    switch (sign)
                    {
                        case 'P':
                            this.pacman = new Pacman(this, x, y);
                            break;
                        case '.':
                        case '$':
                            statusbar.coinsLeft++;
                            break;
                        // the ghosts are not in plan
                        case 'r':
                            RedGhost red = new RedGhost(this, x, y);
                            ghosts.Add(red);
                            plan[x, y] = ' ';
                            break;
                        default:
                            break;
                    }
                }
            }
            sr.Close();
            foreach (Ghost gh in ghosts)
                { gh.pacman = pacman; }
            statusbar.width = width * sx;
        }

        public void MovePacman(int from_x, int from_y, int to_x, int to_y)
        {
            // we suppose the move is valid which is checked by other functions
            char from = plan[from_x, from_y];
            char to = plan[to_x, to_y];
            if (to == '.' || to == '$')
            {
                statusbar.coinsLeft--;
                statusbar.score++;
                if (statusbar.coinsLeft == 0)
                { state = State.win; }
            }
            plan[to_x, to_y] = 'P';
            plan[from_x, from_y] = ' ';
            pacman.x = to_x;
            pacman.y = to_y;

            foreach (Ghost gh in ghosts)
            {
                if (gh.x == to_x && gh.y == to_y)
                    { state = State.loss; }
            }
        }

        public void MoveObjects(KeyPressed key, int tickCounter)
        {
            if (tickCounter % pacman.slowness == 0)
            {
                pacman.Turn(key);
                pacman.Move();
            }

            foreach (Ghost gh in ghosts)
            {
                // have to check twice or they could cross and not be on same coords
                // = the ghosts are smart and stay on the spot if needed
                if (gh.x == pacman.x && gh.y == pacman.y)
                    { state = State.loss; }
                else
                {
                    if (tickCounter % gh.slowness == 0)
                        { gh.Move(); }
                    if (gh.x == pacman.x && gh.y == pacman.y)
                        { state = State.loss; }
                }
            }
        }
    }
}