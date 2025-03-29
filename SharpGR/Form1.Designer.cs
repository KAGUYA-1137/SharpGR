namespace SharpGR
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
        ///  デザイナーのサポートに必要なメソッドです。
        ///  コード エディターでこのメソッドの内容を変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            buttonPlay = new Button();
            trackBarVol = new TrackBar();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            ヘルプToolStripMenuItem = new ToolStripMenuItem();
            操作方法ToolStripMenuItem = new ToolStripMenuItem();
            sharpGRについてToolStripMenuItem = new ToolStripMenuItem();
            numericUpDownVol = new NumericUpDown();
            label2 = new Label();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            Namelabel = new Label();
            Artistlabel = new Label();
            Albumlabel = new Label();
            Yearlabel = new Label();
            Circlelabel = new Label();
            AlbumArtpictureBox = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);
            trackBarDuration = new TrackBar();
            Timelabel = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBarVol).BeginInit();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVol).BeginInit();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AlbumArtpictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBarDuration).BeginInit();
            SuspendLayout();
            // 
            // buttonPlay
            // 
            buttonPlay.Location = new Point(12, 392);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(81, 45);
            buttonPlay.TabIndex = 0;
            buttonPlay.Text = "再生";
            buttonPlay.UseVisualStyleBackColor = true;
            buttonPlay.Click += buttonPlay_Click;
            // 
            // trackBarVol
            // 
            trackBarVol.Location = new Point(61, 32);
            trackBarVol.Maximum = 100;
            trackBarVol.Name = "trackBarVol";
            trackBarVol.Size = new Size(627, 45);
            trackBarVol.TabIndex = 1;
            trackBarVol.Scroll += trackBarVol_Scroll;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 39);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 2;
            label1.Text = "音量：";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { ヘルプToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(787, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // ヘルプToolStripMenuItem
            // 
            ヘルプToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { 操作方法ToolStripMenuItem, sharpGRについてToolStripMenuItem });
            ヘルプToolStripMenuItem.Name = "ヘルプToolStripMenuItem";
            ヘルプToolStripMenuItem.Size = new Size(48, 20);
            ヘルプToolStripMenuItem.Text = "ヘルプ";
            // 
            // 操作方法ToolStripMenuItem
            // 
            操作方法ToolStripMenuItem.Name = "操作方法ToolStripMenuItem";
            操作方法ToolStripMenuItem.Size = new Size(156, 22);
            操作方法ToolStripMenuItem.Text = "操作方法";
            操作方法ToolStripMenuItem.Click += 操作方法ToolStripMenuItem_Click;
            // 
            // sharpGRについてToolStripMenuItem
            // 
            sharpGRについてToolStripMenuItem.Name = "sharpGRについてToolStripMenuItem";
            sharpGRについてToolStripMenuItem.Size = new Size(156, 22);
            sharpGRについてToolStripMenuItem.Text = "SharpGRについて";
            sharpGRについてToolStripMenuItem.Click += sharpGRについてToolStripMenuItem_Click;
            // 
            // numericUpDownVol
            // 
            numericUpDownVol.Location = new Point(694, 37);
            numericUpDownVol.Name = "numericUpDownVol";
            numericUpDownVol.Size = new Size(58, 23);
            numericUpDownVol.TabIndex = 5;
            numericUpDownVol.ValueChanged += numericUpDownVol_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(758, 41);
            label2.Name = "label2";
            label2.Size = new Size(17, 15);
            label2.TabIndex = 2;
            label2.Text = "%";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 440);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(787, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("ＭＳ Ｐゴシック", 9F, FontStyle.Bold, GraphicsUnit.Point, 128);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(772, 17);
            toolStripStatusLabel1.Spring = true;
            // 
            // Namelabel
            // 
            Namelabel.AutoSize = true;
            Namelabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 128);
            Namelabel.Location = new Point(17, 88);
            Namelabel.Name = "Namelabel";
            Namelabel.Size = new Size(0, 17);
            Namelabel.TabIndex = 7;
            // 
            // Artistlabel
            // 
            Artistlabel.AutoSize = true;
            Artistlabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Artistlabel.Location = new Point(17, 115);
            Artistlabel.Name = "Artistlabel";
            Artistlabel.Size = new Size(0, 17);
            Artistlabel.TabIndex = 7;
            // 
            // Albumlabel
            // 
            Albumlabel.AutoSize = true;
            Albumlabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Albumlabel.Location = new Point(17, 139);
            Albumlabel.Name = "Albumlabel";
            Albumlabel.Size = new Size(0, 17);
            Albumlabel.TabIndex = 7;
            // 
            // Yearlabel
            // 
            Yearlabel.AutoSize = true;
            Yearlabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Yearlabel.Location = new Point(17, 163);
            Yearlabel.Name = "Yearlabel";
            Yearlabel.Size = new Size(0, 17);
            Yearlabel.TabIndex = 7;
            // 
            // Circlelabel
            // 
            Circlelabel.AutoSize = true;
            Circlelabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Circlelabel.Location = new Point(17, 187);
            Circlelabel.Name = "Circlelabel";
            Circlelabel.Size = new Size(0, 17);
            Circlelabel.TabIndex = 7;
            // 
            // AlbumArtpictureBox
            // 
            AlbumArtpictureBox.Location = new Point(413, 83);
            AlbumArtpictureBox.Name = "AlbumArtpictureBox";
            AlbumArtpictureBox.Size = new Size(362, 291);
            AlbumArtpictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            AlbumArtpictureBox.TabIndex = 8;
            AlbumArtpictureBox.TabStop = false;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // trackBarDuration
            // 
            trackBarDuration.Enabled = false;
            trackBarDuration.Location = new Point(99, 392);
            trackBarDuration.Name = "trackBarDuration";
            trackBarDuration.Size = new Size(676, 45);
            trackBarDuration.TabIndex = 9;
            // 
            // Timelabel
            // 
            Timelabel.AutoSize = true;
            Timelabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Timelabel.Location = new Point(17, 359);
            Timelabel.Name = "Timelabel";
            Timelabel.Size = new Size(0, 17);
            Timelabel.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(787, 462);
            Controls.Add(trackBarDuration);
            Controls.Add(AlbumArtpictureBox);
            Controls.Add(Timelabel);
            Controls.Add(Circlelabel);
            Controls.Add(Yearlabel);
            Controls.Add(Albumlabel);
            Controls.Add(Artistlabel);
            Controls.Add(Namelabel);
            Controls.Add(statusStrip1);
            Controls.Add(numericUpDownVol);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(trackBarVol);
            Controls.Add(buttonPlay);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            ImeMode = ImeMode.Disable;
            KeyPreview = true;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            ((System.ComponentModel.ISupportInitialize)trackBarVol).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVol).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AlbumArtpictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBarDuration).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem ヘルプToolStripMenuItem;
        private ToolStripMenuItem 操作方法ToolStripMenuItem;
        public TrackBar trackBarVol;
        public Button buttonPlay;
        private ToolStripMenuItem sharpGRについてToolStripMenuItem;
        private NumericUpDown numericUpDownVol;
        private Label label2;
        private StatusStrip statusStrip1;
        public ToolStripStatusLabel toolStripStatusLabel1;
        public Label Namelabel;
        public Label Artistlabel;
        public Label Albumlabel;
        public Label Yearlabel;
        public Label Circlelabel;
        public PictureBox AlbumArtpictureBox;
        private System.Windows.Forms.Timer timer1;
        public TrackBar trackBarDuration;
        public Label Timelabel;
    }
}
