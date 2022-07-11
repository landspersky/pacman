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

        private void bPlay_Click(object sender, EventArgs e)
        {
            g = CreateGraphics();
            map = new Map(@"C:\Users\admin\source\repos\Pacman\Pacman\plan.txt", 
                @"C:\Users\admin\source\repos\Pacman\Pacman\basic_icons.png", this);

            bPlay.Visible = false;
            bSettings.Visible = false;
            lAuthor.Visible = false;
            lTitle.Visible = false;

            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            switch (map.state)
            {
                case State.running:
                    map.Draw(g, ClientSize.Width, ClientSize.Height);
                    break;
                // win & loss scenarios
                default:
                    break;
            }
        }
    }
}