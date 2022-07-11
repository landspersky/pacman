namespace Pacman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

            bPlay.Visible = false;
            bSettings.Visible = false;
            lAuthor.Visible = false;
            lTitle.Visible = false;

            timer.Enabled = true;
            lLives.Visible = true;
            lScore.Visible = true;
            bMenu.Visible = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (map.state)
            {
                case State.running:
                    map.Draw(g, ClientSize.Width, ClientSize.Height);
                    statusBar.Draw();
                    break;
                // win & loss scenarios
                default:
                    break;
            }
        }
    }
}