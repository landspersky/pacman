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
        public (int, int) location;
        public abstract void Move();

        protected Character(Map map, int x, int y)
        {
            this.map = map;
            this.location = (x, y);
        }

        protected (int, int) TargetCoords(Direction direction, (int, int) coords)
        {
            // returns (x, y) of the field that is 'direction' of (from_x, from_y)
            int from_x = coords.Item1;
            int from_y = coords.Item2;
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
            return TargetCoords(direction, location);
        }

        protected bool IsFreeSpace(Direction direction, (int, int) coords)
        {
            return map.IsFreeSpace(TargetCoords(direction, coords));
        }

        protected bool IsFreeSpace(Direction direction)
        {
            return IsFreeSpace(direction, location);
        }
    }
    abstract class Ghost : Character
    {
        public char id;
        public bool enabled = false;
        private (int, int) lastLocation = (0, 0);
        public Pacman pacman;
        private Random generator = new Random();

        protected Ghost(Map map, int x, int y) : base(map, x, y)
        {
        }

        protected (int, int) NextOnShortest((int, int) to)
        {
            // returns the next location on the shortest path to (to_x, to_y)
            if (location == to)
                { return location; }

            Queue<(int, int)> q = new Queue<(int, int)>();
            q.Enqueue(location);
            Dictionary<(int, int), (int, int)> firstParent = new Dictionary<(int, int), (int, int)>();
            (int, int) current = location;

            // finds the closest free spot
            // allows us to find paths to spots with walls
            if (! map.IsFreeSpace(current))
            {
                List<(int, int)> visited = new List<(int, int)>();
                visited.Add(current);
                while ( true )
                {
                    current = q.Dequeue();
                    if (map.IsFreeSpace(current))
                        { break; }
                    foreach (Direction d in Enum.GetValues(typeof(Direction)))
                    {
                        (int, int) neighbour = TargetCoords(d, current);
                        if (!visited.Contains(neighbour) && !OutsideOfMap(neighbour))
                        {
                            q.Enqueue(neighbour);
                            visited.Add(neighbour);
                        }
                    }
                    
                }
                to = current;
                q.Clear();
                q.Enqueue(location);
            }

            while (! firstParent.ContainsKey(to))
            {
                // when the location is in inaccessible, eg ghost box
                if (q.Count == 0)
                    { return location; }
                current = q.Dequeue();
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    (int, int) neighbour = TargetCoords(d, current);
                    if (! firstParent.ContainsKey(neighbour) && map.IsFreeSpace(neighbour) && ! OutsideOfMap(neighbour))
                    {
                        q.Enqueue(neighbour);
                        if (firstParent.ContainsKey(current))
                            { firstParent[neighbour] = firstParent[current]; }
                        else
                            { firstParent[neighbour] = neighbour; }
                    }
                }
            }
            return firstParent[to];
        }

        protected bool OutsideOfMap((int, int) coords)
        {
            // outside OR on the border
            return coords.Item1 < 1 || coords.Item1 > map.width - 2 || coords.Item2 < 1 || coords.Item2 > map.height - 2;
        }

        public double Distance((int, int) coords1, (int, int) coords2)
        {
           return Math.Sqrt(Math.Pow(Math.Abs(coords1.Item1 - coords2.Item1), 2) +
                Math.Pow(Math.Abs(coords1.Item2 - coords2.Item2), 2));
        }

        public double Distance((int, int) coords)
        {
            return Distance(coords, location);
        }

        private void MoveRandomly()
        {
            // moves randomly, preferably to a location different to last one
            List<(int, int)> possibleMoves = new List<(int, int)>();
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                (int, int) neighbour = TargetCoords(d);
                if ( map.IsFreeSpace(neighbour) && neighbour != lastLocation)
                    { possibleMoves.Add(neighbour); }
            }
            if (possibleMoves.Count == 0) 
                { location = lastLocation; }
            else
                { location = possibleMoves[generator.Next(possibleMoves.Count)]; }


        }
        public void MoveGhost()
        {
            (int, int) temp = location;
            if (! map.frightenedMode )
                { Move(); }
            else
                { MoveRandomly(); }
            lastLocation = temp;
        }
    }

    class RedGhost : Ghost
    {
        public RedGhost(Map map, int x, int y) : base(map, x, y)
        {
            id = 'r';
            slowness = 12;
        }

        public override void Move()
        {
            // chases after pacman
            (int, int) to = NextOnShortest(pacman.location);
            location = to;
        }
    }

    class PinkGhost : Ghost
    {
        public PinkGhost(Map map, int x, int y) : base(map, x, y)
        {
            id = 'p';
            slowness = 10;
        }

        public override void Move()
        {
            // wants to get ahead of pacman

            // get coords of field two steps ahead of pacman
            Direction d = pacman.direction;
            (int, int) temp = TargetCoords(d, pacman.location);
            temp = TargetCoords(d, temp);

            if (OutsideOfMap(temp))
            { 
                // we look at the opposite direction (suppose the correct order of enum)
                d = (Direction)(((int)d + 2) % 4);
                temp = TargetCoords(d, pacman.location);
                temp = TargetCoords(d, temp);
            }

            while (! map.IsFreeSpace(temp))
                { temp = TargetCoords(d, temp); }

            location = NextOnShortest(temp);
        }
    }

    class OrangeGhost : Ghost
    {
        public OrangeGhost(Map map, int x, int y) : base(map, x, y)
        {
            id = 'o';
            slowness = 10;
            corners = new (int, int)[] { (1, 1), (map.width - 2, 1), 
                (map.width - 2, map.height - 2), (1, map.height - 2) };
        }

        private (int, int)[] corners;
        private bool bloodthirsty = false;
        private int cornerIndex = 0;
        private int steps = 0;

        public override void Move()
        {
            // chases after pacman unless too close, then goes to corner for a while

            double distance = Distance(pacman.location);
            if (distance < 5)
                { bloodthirsty = false; }
            if ( location == corners[cornerIndex] || steps == 10)
            {
                bloodthirsty = true;
                cornerIndex = (cornerIndex + 1) % 4;
                steps = 0;
            }
            (int, int) to;
            if (bloodthirsty)
                { to = NextOnShortest(pacman.location); }
            else
            { 
                to = NextOnShortest(corners[cornerIndex]);
                steps++;
            }
            location = to;
        }
    }

    class BlueGhost : Ghost
    {
        public BlueGhost(Map map, int x, int y): base(map, x, y)
        {
            id = 'b';
            slowness = 5;
        }

        public List<Ghost> ghosts = new List<Ghost>();

        public override void Move()
        {
            // goes to the center of gravity of other characters flipped by diagonal
            int center_x = 0;
            int center_y = 0;
            foreach (Ghost gh in ghosts)
            {
                if (gh != this)
                {
                    center_x += gh.location.Item1;
                    center_y += gh.location.Item2;
                }
            }
            center_x = (center_x + pacman.location.Item1) / ghosts.Count;
            center_y = (center_y + pacman.location.Item2) / ghosts.Count;
            int midx = map.width / 2;
            int midy = map.height / 2;


            (int, int) to = NextOnShortest((midx + midx - center_x, midy + midy - center_y));
            location = to;
        }
    }

    class Pacman : Character
    {
        public bool opened; // altering between two icons
        public Direction direction = Direction.right;

        public Pacman(Map map, int x, int y) : base(map, x, y)
        {
            slowness = 3;
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
                map.MovePacman(location, new_coords);
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
        private (int, int) spawn;
        public bool frightenedMode = false;
        public int width;
        public int height;
        public int startx;
        public int starty;

        public State state = State.running;

        Bitmap[] icons;
        public int sx;

        public Pacman pacman;
        public StatusBar statusbar;
        private GameTimer timer;
        public List<Ghost> ghosts;
        private Queue<Ghost> disabledGhosts;

        public Map(Form1 form, string mapPath, string iconsPath, StatusBar statusBar, GameTimer timerGame)
        {
            this.statusbar = statusBar;
            this.timer = timerGame;
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
                        indexObrazku += 9 + 2 * (int)pacman.direction;
                    }
                    else 
                        { indexObrazku = " X.$".IndexOf(c); }
                    g.DrawImage(icons[indexObrazku], x * sx + startx, y * sx + starty);
                }
            }

            foreach (Ghost gh in ghosts)
            {
                int indexObrazku;
                if (frightenedMode)
                { 
                    indexObrazku = 8; //specific to this image; edit if necessary
                }
                else
                    { indexObrazku = "rpbo".IndexOf(gh.id) + 4; }
                g.DrawImage(icons[indexObrazku], gh.location.Item1 * sx + startx, 
                    gh.location.Item2 * sx + starty);
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
            disabledGhosts = new Queue<Ghost>();
            StreamReader sr = new StreamReader(path);
            width = int.Parse(sr.ReadLine());
            height = int.Parse(sr.ReadLine());
            plan = new char[width, height];

            BlueGhost blue = new BlueGhost(this, 0, 0);
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
                            red.enabled = true;
                            ghosts.Add(red);
                            spawn = (x, y);
                            plan[x, y] = ' ';
                            break;
                        case 'p':
                            PinkGhost pink = new PinkGhost(this, x, y);
                            ghosts.Add(pink);
                            disabledGhosts.Enqueue(pink);
                            plan[x, y] = ' ';
                            break;
                        case 'o':
                            OrangeGhost orange = new OrangeGhost(this, x, y);
                            ghosts.Add(orange);
                            disabledGhosts.Enqueue(orange);
                            plan[x, y] = ' ';
                            break;
                        case 'b':
                            blue.location = (x, y);
                            ghosts.Add(blue);
                            disabledGhosts.Enqueue(blue);
                            plan[x, y] = ' ';
                            break;
                        default:
                            break;
                    }
                }
            }
            sr.Close();
            foreach (Ghost gh in ghosts)
            { 
                gh.pacman = pacman;
            }
            blue.ghosts = ghosts;
            statusbar.width = width * sx;
        }

        public void MovePacman((int, int) from, (int, int) to)
        {
            // we suppose the move is valid which is checked by other functions
            char to_item = plan[to.Item1, to.Item2];
            if ( to_item != ' ') // it's a coin
            {
                statusbar.coinsLeft--;
                statusbar.score++;
                if (statusbar.coinsLeft == 0)
                { state = State.win; }
                if ( to_item == '$')
                {
                    statusbar.score += 9;
                    frightenedMode = true;
                    timer.lastFrightened = timer.ticks;
                }
            }
            plan[to.Item1, to.Item2] = 'P';
            plan[from.Item1, from.Item2] = ' ';
            pacman.location = to;

            foreach (Ghost gh in ghosts)
            {
                if (gh.location == pacman.location)
                    { state = State.loss; }
            }
        }

        public void MoveObjects(KeyPressed key)
        {
            if (timer.ticks % pacman.slowness == 0)
            {
                pacman.Turn(key);
                pacman.Move();
            }

            if (timer.ticks % timer.enablePeriod == 0 && disabledGhosts.Count != 0)
            {
                Ghost next = disabledGhosts.Dequeue();
                next.enabled = true;
                next.location = spawn;
            }
            foreach (Ghost gh in ghosts)
            {
                // have to check twice or they could cross and not be on same coords
                // = the ghosts are smart and stay on the spot if needed
                if (gh.location == pacman.location)
                    { state = State.loss; }
                else if (gh.enabled)
                {
                    if (timer.ticks % gh.slowness == 0)
                        { gh.Move(); }
                    if (gh.location == pacman.location)
                        { state = State.loss; }
                }
            }
        }
    }
}