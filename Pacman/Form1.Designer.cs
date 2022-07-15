namespace Pacman
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.bPlay = new System.Windows.Forms.Button();
            this.lTitle = new System.Windows.Forms.Label();
            this.lAuthor = new System.Windows.Forms.Label();
            this.bSettings = new System.Windows.Forms.Button();
            this.timerGame = new GameTimer(this.components);
            this.lLives = new System.Windows.Forms.Label();
            this.lScore = new System.Windows.Forms.Label();
            this.bMenu = new System.Windows.Forms.Button();
            this.timerMenu = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // bPlay
            // 
            this.bPlay.Location = new System.Drawing.Point(350, 300);
            this.bPlay.Name = "bPlay";
            this.bPlay.Size = new System.Drawing.Size(75, 23);
            this.bPlay.TabIndex = 2;
            this.bPlay.Text = "PLAY";
            this.bPlay.UseVisualStyleBackColor = true;
            this.bPlay.Click += new System.EventHandler(this.bPlay_Click);
            // 
            // lTitle
            // 
            this.lTitle.AutoSize = true;
            this.lTitle.Font = new System.Drawing.Font("Segoe UI", 64F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lTitle.ForeColor = System.Drawing.Color.Gold;
            this.lTitle.Location = new System.Drawing.Point(191, 107);
            this.lTitle.Name = "lTitle";
            this.lTitle.Size = new System.Drawing.Size(392, 114);
            this.lTitle.TabIndex = 3;
            this.lTitle.Text = "PACMAN";
            // 
            // lAuthor
            // 
            this.lAuthor.AutoSize = true;
            this.lAuthor.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lAuthor.Location = new System.Drawing.Point(440, 221);
            this.lAuthor.Name = "lAuthor";
            this.lAuthor.Size = new System.Drawing.Size(121, 15);
            this.lAuthor.TabIndex = 4;
            this.lAuthor.Text = "by Jakub Landsperský";
            // 
            // bSettings
            // 
            this.bSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bSettings.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSettings.Image = ((System.Drawing.Image)(resources.GetObject("bSettings.Image")));
            this.bSettings.Location = new System.Drawing.Point(743, 9);
            this.bSettings.Margin = new System.Windows.Forms.Padding(0);
            this.bSettings.Name = "bSettings";
            this.bSettings.Size = new System.Drawing.Size(48, 47);
            this.bSettings.TabIndex = 5;
            this.bSettings.UseVisualStyleBackColor = true;
            // 
            // timerGame
            // 
            this.timerGame.Interval = 30;
            this.timerGame.Tick += new System.EventHandler(this.timerGame_Tick);
            // 
            // lLives
            // 
            this.lLives.AutoSize = true;
            this.lLives.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lLives.Location = new System.Drawing.Point(219, 67);
            this.lLives.Name = "lLives";
            this.lLives.Size = new System.Drawing.Size(45, 15);
            this.lLives.TabIndex = 6;
            this.lLives.Text = "Lives: 3";
            // 
            // lScore
            // 
            this.lScore.AutoSize = true;
            this.lScore.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lScore.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lScore.Location = new System.Drawing.Point(403, 57);
            this.lScore.Name = "lScore";
            this.lScore.Size = new System.Drawing.Size(22, 25);
            this.lScore.TabIndex = 7;
            this.lScore.Text = "0";
            // 
            // bMenu
            // 
            this.bMenu.Location = new System.Drawing.Point(565, 63);
            this.bMenu.Name = "bMenu";
            this.bMenu.Size = new System.Drawing.Size(75, 23);
            this.bMenu.TabIndex = 8;
            this.bMenu.Text = "Menu";
            this.bMenu.UseVisualStyleBackColor = true;
            this.bMenu.Click += new System.EventHandler(this.bMenu_Click);
            // 
            // timerMenu
            // 
            this.timerMenu.Enabled = true;
            this.timerMenu.Tick += new System.EventHandler(this.timerMenu_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(800, 661);
            this.Controls.Add(this.bMenu);
            this.Controls.Add(this.lScore);
            this.Controls.Add(this.lLives);
            this.Controls.Add(this.bSettings);
            this.Controls.Add(this.lAuthor);
            this.Controls.Add(this.lTitle);
            this.Controls.Add(this.bPlay);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "PACMAN";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Button bPlay;
        private Label lTitle;
        private Label lAuthor;
        private Button bSettings;
        private GameTimer timerGame;
        private Label lLives;
        private Label lScore;
        private Button bMenu;
        private System.Windows.Forms.Timer timerMenu;
    }
}