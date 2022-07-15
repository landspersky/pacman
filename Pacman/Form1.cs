using System.ComponentModel;

namespace Pacman
{
    public partial class Form1 : Form
    {
        public enum Screen { Menu, Game };

        Map map;
        StatusBar statusBar;
        KeyPressed keyPressed;
        Size size;
        bool frozen;
        private int level;
        private BufferedGraphicsContext context;
        private BufferedGraphics grafx;

        public Form1()
        {
            size = this.Size;
            InitializeComponent();

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            grafx = context.Allocate(this.CreateGraphics(),
                new Rectangle(0, 0, this.Width, this.Height));

            initializeScreen(Screen.Menu);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            grafx.Render(e.Graphics);
        }

        private void initializeScreen(Screen screen, int lives, int score)
        {
            bool toGame;
            switch (screen)
            {
                case Screen.Game:
                    toGame = true;
                    timerMenu.Enabled = false;
                    this.statusBar = new StatusBar(lLives, lScore, bMenu, lives, score);
                    map = new Map(this, @"C:\Users\admin\source\repos\Pacman\Pacman\plan.txt",
                        @"C:\Users\admin\source\repos\Pacman\Pacman\basic_icons.png", statusBar,
                        timerGame);
                    timerGame.Enabled = true;
                    break;
                case Screen.Menu:
                    toGame = false;
                    timerGame.Enabled = false;
                    eraseScreen();
                    timerMenu.Enabled = true;
                    break;
                default:
                    toGame = false;
                    break;
            }
            timerGame.ticks = 0;
            keyPressed = KeyPressed.none;
            bPlay.Visible = !toGame;
            bSettings.Visible = !toGame;
            lAuthor.Visible = !toGame;
            lTitle.Visible = !toGame;

            lLives.Visible = toGame;
            lScore.Visible = toGame;
            bMenu.Visible = toGame;
        }

        private void initializeScreen(Screen screen)
            { initializeScreen(screen, 3, 0); }
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
            grafx.Graphics.FillRectangle(blackBrush, rect);
            Refresh();
        }

        private void bPlay_Click(object sender, EventArgs e)
        {
            initializeScreen(Screen.Game);
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            switch (map.state)
            {
                case State.running:
                    // if the user is changing window size, we don't draw anything
                    // after adjustment, we need to wipe the screen
                    if (size == this.Size)
                    {
                        if (frozen)
                        {
                            eraseScreen();
                            frozen = false;
                        }
                        timerGame.ticks++;
                        if (timerGame.ticks - timerGame.lastFrightened == timerGame.frightenedPeriod)
                            { map.frightenedMode = false; }
                        map.MoveObjects(keyPressed);
                        map.Draw(grafx.Graphics, ClientSize.Width, ClientSize.Height);
                        Refresh();
                        statusBar.Draw(map);
                    }
                    else
                        { frozen = true; }
                    break;
                case State.win:
                    timerGame.Enabled = false;
                    if (level == 3)
                    {
                        MessageBox.Show("Great. I think you've had enough," +
                            " please do something else, the Pacman is tired and needs to take a break.");
                        initializeScreen(Screen.Menu);
                    }
                    else
                    { 
                        timerGame.Reset();
                        initializeScreen(Screen.Game, statusBar.livesLeft, statusBar.score);
                        level++;
                    }
                    break;
                case State.loss:
                    timerGame.Enabled = false;
                    if (statusBar.livesLeft == 0)
                    {
                        MessageBox.Show("You lost!");
                        initializeScreen(Screen.Menu);
                    }
                    else
                    {
                        timerGame.Reset();
                        map.Reset();
                        map.state = State.running;
                        Thread.Sleep(1000);
                        timerGame.Enabled = true;
                    }
                    break;
                default:
                    break;
            }
            size = this.Size;
        }

        private void timerMenu_Tick(object sender, EventArgs e)
        {
            drawMenuScreen();
        }

        private void bMenu_Click(object sender, EventArgs e)
        {
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

    public class GameTimer : System.Windows.Forms.Timer
    {
        public int ticks = 0;
        public int lastEnabled = 0;
        public int enablePeriod = 100;
        public int lastFrightened = 0;
        public int frightenedPeriod = 300;
        public GameTimer(IContainer container) : base(container)
        {
        }

        public void Reset()
        {
            ticks = 0;
            lastEnabled = 0;
            lastFrightened = 0;
        }
    }
}