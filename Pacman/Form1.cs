namespace Pacman
{
    public partial class Form1 : Form
    {
        public enum Screen { Menu, Game };

        public Form1()
        {
            InitializeComponent();
            initializeScreen(Screen.Menu);

            timer.Enabled = true;
        }

        private void initializeScreen(Screen screen)
        {
            bool toGame;
            switch (screen)
            {
                case Screen.Game:
                    toGame = true;
                    break;
                default:
                    toGame = false;
                    break;
            }
            bPlay.Visible = !toGame;
            bSettings.Visible = !toGame;
            lAuthor.Visible = !toGame;
            lTitle.Visible = !toGame;

            lLives.Visible = toGame;
            lScore.Visible = toGame;
            bMenu.Visible = toGame;
        }

        private void drawMenuScreen()
        {
            int midx = ClientSize.Width / 2;
            int padding = 10;

            lTitle.Left = midx - lTitle.Width / 2;
            lTitle.Top = ClientSize.Height / 3 - lTitle.Height / 2;

            lAuthor.Left = midx;
            lAuthor.Top = lTitle.Top + lTitle.Height + padding;

            bPlay.Left = midx - bPlay.Width / 2;
            bPlay.Top = ClientSize.Height * 3 / 5;

            bSettings.Left = ClientSize.Width - bSettings.Width - padding;
            bSettings.Top = padding;
        }

        private void eraseScreen()
        {
            // Make the screen black
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            Rectangle rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            g.FillRectangle(blackBrush, rect);
        }

        Map map;
        Graphics g;
        StatusBar statusBar;
        Point coords;
        KeyPressed keyPressed = KeyPressed.none;

        private void bPlay_Click(object sender, EventArgs e)
        {
            g = CreateGraphics();
            this.statusBar = new StatusBar(lLives, lScore, bMenu);
            map = new Map(this, @"C:\Users\admin\source\repos\Pacman\Pacman\plan.txt",
                @"C:\Users\admin\source\repos\Pacman\Pacman\basic_icons.png", statusBar);
            map.state = State.running;
            initializeScreen(Screen.Game);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (map.state)
            {
                case State.running:
                    map.MoveObjects(keyPressed);
                    if (this.Location != coords)
                    {
                        eraseScreen();
                    }
                    else
                    {
                        map.Draw(g, ClientSize.Width, ClientSize.Height);
                        statusBar.Draw(map);
                    }
                    break;
                case State.idle:
                    drawMenuScreen();
                    break;
                // win & loss scenarios
                default:
                    break;
            }
            coords = this.Location;
        }

        private void bMenu_Click(object sender, EventArgs e)
        {
            eraseScreen();
            map.state = State.idle;
            initializeScreen(Screen.Menu);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                keyPressed = KeyPressed.up;
                return true;
            }
            if (keyData == Keys.Down)
            {
                keyPressed = KeyPressed.down;
                return true;
            }
            if (keyData == Keys.Left)
            {
                keyPressed = KeyPressed.left;
                return true;
            }
            if (keyData == Keys.Right)
            {
                keyPressed = KeyPressed.right;
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            keyPressed = KeyPressed.none;
        }
    }
}