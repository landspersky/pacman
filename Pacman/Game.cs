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
        // other atributes

        public Map(string mapPath, string iconsPath, Form form)
        {
            LoadIcons(iconsPath);
            LoadMap(mapPath);
            state = State.running;
            form.MinimumSize = new Size(width * sx + 2 * padding, height * sx * 7 / 6 + 2 * padding);
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

                        default:
                            break;
                    }
                }
            }
            sr.Close();
        }
    }
}