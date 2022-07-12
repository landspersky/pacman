namespace Pacman
{
    public partial class Form1 : Form
    {
        public enum Screen { Menu, Game };

        public Form1()
        {
            InitializeComponent();
            initializeScreen(Screen.Menu);
        }

        public void initializeScreen(Screen screen)
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

            timer.Enabled = toGame;
            lLives.Visible = toGame;
            lScore.Visible = toGame;
            bMenu.Visible = toGame;
        }

        Map map;
        Graphics g;
        StatusBar statusBar;

        private void bPlay_Click(object sender, EventArgs e)
        {
            g = CreateGraphics();
            this.statusBar = new StatusBar(lLives, lScore, bMenu);
            map = new Map(this, @"C:\Users\admin\source\repos\Pacman\Pacman\plan.txt", 
                @"C:\Users\admin\source\repos\Pacman\Pacman\basic_icons.png", statusBar);

            initializeScreen(Screen.Game);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (map.state)
            {
                case State.running:
                    map.Draw(g, ClientSize.Width, ClientSize.Height);
                    statusBar.Draw(map);
                    break;
                // win & loss scenarios
                default:
                    break;
            }
        }

        private void bMenu_Click(object sender, EventArgs e)
        {
            // Make the window black
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            Rectangle rect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);
            g.FillRectangle(blackBrush, rect);

            initializeScreen(Screen.Menu);
        }
    }
}