using System;
using System.Drawing;

namespace Pacman
{
    public enum KeyPressed { none, left, up, right, down };

    public enum Direction {  left, up, right, down };

    class Pacman
    {
        private Map map;
        public int x;
        public int y;
        private bool opened; // altering between two icons
        private Direction direction = Direction.left;

        public Pacman(Map map, int x, int y)
        {
            this.map = map;
            this.x = x;
            this.y = y;
        }

        private (int, int) TargetCoords(Direction direction)
        {
            // returns (x, y) of the field that is 'direction' of pacman
            int target_x = x;
            int target_y = y;
            switch (direction)
            {
                case Direction.left:
                    target_x--;
                    break;
                case Direction.up:
                    target_y--;
                    break;
                case Direction.right:
                    target_x++;
                    break;
                case Direction.down:
                    target_y++;
                    break;
                default:
                    break;
            }
            return (target_x, target_y);
        }
        
        private bool IsFreeSpace(Direction direction)
        {
            var coords = TargetCoords(direction);
            return map.IsFreeSpace(coords.Item1, coords.Item2);
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

        public void Move()
        {
            (int, int) new_coords = TargetCoords(direction);

            if (map.IsFreeSpace(new_coords.Item1, new_coords.Item2))
            {
                map.Move(x, y, new_coords.Item1, new_coords.Item2);
            }
        }
    }

    class StatusBar
    {
        public int coinsLeft;
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

    public enum State { idle, running };

    class Map
    {
        private char[,] plan;
        public int width;
        public int height;
        public int startx;
        public int starty;

        public State state = State.idle;

        Bitmap[] icons;
        public int sx;

        public Pacman pacman;
        public StatusBar statusbar;
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

        public bool IsFreeSpace(int x, int y)
        {
            return plan[x, y] != 'X';
        }

        public void Draw(Graphics g, int windowWidth, int windowHeight)
        {
            int midx = windowWidth / 2;
            int midy = windowHeight / 2;
            startx = midx - width * sx / 2;
            starty = midy - height * sx / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char c = plan[x, y];
                    int indexObrazku = " X.$P".IndexOf(c);
                    g.DrawImage(icons[indexObrazku], x * sx + startx, y * sx + starty);
                }
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
            StreamReader sr = new StreamReader(path);
            width = int.Parse(sr.ReadLine());
            height = int.Parse(sr.ReadLine());
            plan = new char[width, height];

            int coins_count = 0;
            for (int y = 0; y < height; y++)
            {
                string line = sr.ReadLine();
                for (int x = 0; x < width; x++)
                {
                    char znak = line[x];
                    plan[x, y] = znak;

                    switch (znak)
                    {
                        case 'P':
                            this.pacman = new Pacman(this, x, y);
                            break;

                        case '.':
                        case '$':
                            coins_count++;
                            break;
                        default:
                            break;
                    }
                }
            }
            sr.Close();
            statusbar.coinsLeft = coins_count;
            statusbar.width = width * sx;
        }

        public void Move(int from_x, int from_y, int to_x, int to_y)
        {
            // First version: we suppose we're moving Pacman

            char to = plan[to_x, to_y];
            // TODO: if it's a ghost moving, a coin must reveal itself
            // if there is a ghost, the game ends
            if (to == '.' || to == '$')
            {
                statusbar.coinsLeft--;
                statusbar.score++;
            }
            plan[to_x, to_y] = 'P';
            plan[from_x, from_y] = ' ';
            pacman.x = to_x;
            pacman.y = to_y;
        }
        public void MoveObjects(KeyPressed key)
        {
            pacman.Turn(key);
            pacman.Move();
        }
    }
}