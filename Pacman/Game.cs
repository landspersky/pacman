using System;
using System.Drawing;

namespace Pacman
{
    class Pacman
    {
        public Map map;
        public int x;
        public int y;
        public int smer;
        public bool opened; // altering between two icons

        public Pacman(Map map, int x, int y)
        {
            this.map = map;
            this.x = x;
            this.y = y;
        }
    }

    class StatusBar
    {
        public int coinsLeft;
        public int livesLeft;
        public int score;
        public int width;
        public int height = 30;
        public int startx;
        public int starty;
        private Label lLives;
        private Label lScore;
        private Button bMenu;

        public StatusBar(Label lLives, Label lScore, Button bMenu)
        {
            this.lLives = lLives;
            this.lScore = lScore;
            this.bMenu = bMenu;
        }

        public void Draw()
        {
            int midy = starty + height / 2;
            int midx = startx + width / 2;

            // Lives
            lLives.Text = $"Lives: {livesLeft}";
            lLives.Left = startx + 10;
            lLives.Top = starty + 10;

            // Score
            lScore.Text = $"{score}";
            lScore.Left = midx - lScore.Width / 2;
            lScore.Top = midy - lScore.Height / 2;

            // Menu
            bMenu.Top = starty + 10;
            bMenu.Left = startx + width - 10 - bMenu.Width / 2;

        }
    }

    public enum State { idle, running };

    class Map
    {
        private char[,] plan;
        int width;
        int height;
        int padding = 10;

        public State state = State.idle;

        Bitmap[] icons;
        int sx;

        public Pacman pacman;
        public StatusBar statusbar;
        // other atributes

        public Map(Form form, string mapPath, string iconsPath, StatusBar statusBar)
        {
            this.statusbar = statusBar;

            form.MinimumSize = new Size(width * sx + 2 * padding,
                height * sx + statusbar.height + 2 * padding);
            LoadIcons(iconsPath);
            LoadMap(mapPath);
            state = State.running;

        }

        public void Draw(Graphics g, int windowWidth, int windowHeight)
        {
            int midx = windowWidth / 2;
            int midy = windowHeight / 2;
            int startx = midx - width * sx / 2;
            int starty = midy - height * sx / 2;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    char c = plan[x, y];
                    int indexObrazku = " X.$P".IndexOf(c);
                    g.DrawImage(icons[indexObrazku], x * sx + startx, y * sx + starty);
                }
            }

            statusbar.startx = startx;
            statusbar.starty = starty - statusbar.height;
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
    }
}