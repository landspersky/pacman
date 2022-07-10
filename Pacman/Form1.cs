namespace Pacman
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Map map;

        private void bPlay_Click(object sender, EventArgs e)
        {
            map = new Map("plan.txt", "icons.png");

            bPlay.Visible = false;
            bSettings.Visible = false;
            lAuthor.Visible = false;
            lTitle.Visible = false;
        }
    }
}