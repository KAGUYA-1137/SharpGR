﻿namespace SharpGR
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
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
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
            buttonPlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonPlay.Location = new Point(12, 458);
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
            trackBarVol.Size = new Size(677, 45);
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
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.Items.AddRange(new ToolStripItem[] { ヘルプToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(56, 24);
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
            numericUpDownVol.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            numericUpDownVol.Location = new Point(744, 37);
            numericUpDownVol.Name = "numericUpDownVol";
            numericUpDownVol.Size = new Size(58, 23);
            numericUpDownVol.TabIndex = 5;
            numericUpDownVol.ValueChanged += numericUpDownVol_ValueChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(808, 41);
            label2.Name = "label2";
            label2.Size = new Size(17, 15);
            label2.TabIndex = 2;
            label2.Text = "%";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 506);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(837, 22);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("ＭＳ Ｐゴシック", 9F, FontStyle.Bold, GraphicsUnit.Point, 128);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(822, 17);
            toolStripStatusLabel1.Spring = true;
            // 
            // Namelabel
            // 
            Namelabel.AutoSize = true;
            Namelabel.Font = new Font("Yu Gothic UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 128);
            Namelabel.Location = new Point(325, 113);
            Namelabel.Name = "Namelabel";
            Namelabel.Size = new Size(33, 20);
            Namelabel.TabIndex = 7;
            Namelabel.Text = "----";
            // 
            // Artistlabel
            // 
            Artistlabel.AutoSize = true;
            Artistlabel.Font = new Font("Yu Gothic UI", 11.25F);
            Artistlabel.Location = new Point(325, 142);
            Artistlabel.Name = "Artistlabel";
            Artistlabel.Size = new Size(33, 20);
            Artistlabel.TabIndex = 7;
            Artistlabel.Text = "----";
            // 
            // Albumlabel
            // 
            Albumlabel.AutoSize = true;
            Albumlabel.Font = new Font("Yu Gothic UI", 11.25F);
            Albumlabel.Location = new Point(325, 171);
            Albumlabel.Name = "Albumlabel";
            Albumlabel.Size = new Size(33, 20);
            Albumlabel.TabIndex = 7;
            Albumlabel.Text = "----";
            // 
            // Yearlabel
            // 
            Yearlabel.AutoSize = true;
            Yearlabel.Font = new Font("Yu Gothic UI", 11.25F);
            Yearlabel.Location = new Point(297, 220);
            Yearlabel.Name = "Yearlabel";
            Yearlabel.Size = new Size(37, 20);
            Yearlabel.TabIndex = 7;
            Yearlabel.Text = "20--";
            Yearlabel.Visible = false;
            // 
            // Circlelabel
            // 
            Circlelabel.AutoSize = true;
            Circlelabel.Font = new Font("Yu Gothic UI", 11.25F);
            Circlelabel.Location = new Point(297, 200);
            Circlelabel.Name = "Circlelabel";
            Circlelabel.Size = new Size(33, 20);
            Circlelabel.TabIndex = 7;
            Circlelabel.Text = "----";
            Circlelabel.Visible = false;
            // 
            // AlbumArtpictureBox
            // 
            AlbumArtpictureBox.Anchor = AnchorStyles.Right;
            AlbumArtpictureBox.Location = new Point(17, 113);
            AlbumArtpictureBox.Name = "AlbumArtpictureBox";
            AlbumArtpictureBox.Size = new Size(256, 252);
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
            trackBarDuration.Anchor = AnchorStyles.Bottom;
            trackBarDuration.Enabled = false;
            trackBarDuration.Location = new Point(99, 458);
            trackBarDuration.Name = "trackBarDuration";
            trackBarDuration.Size = new Size(726, 45);
            trackBarDuration.TabIndex = 9;
            // 
            // Timelabel
            // 
            Timelabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            Timelabel.AutoSize = true;
            Timelabel.Font = new Font("Yu Gothic UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 128);
            Timelabel.Location = new Point(17, 428);
            Timelabel.Name = "Timelabel";
            Timelabel.Size = new Size(67, 17);
            Timelabel.TabIndex = 7;
            Timelabel.Text = "--:-- / --:--";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Yu Gothic UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 128);
            label3.Location = new Point(297, 113);
            label3.Name = "label3";
            label3.Size = new Size(24, 20);
            label3.TabIndex = 7;
            label3.Text = "♪";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Yu Gothic UI", 11.25F);
            label4.Location = new Point(297, 142);
            label4.Name = "label4";
            label4.Size = new Size(26, 20);
            label4.TabIndex = 7;
            label4.Text = "👤";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Yu Gothic UI", 11.25F);
            label5.Location = new Point(297, 171);
            label5.Name = "label5";
            label5.Size = new Size(29, 20);
            label5.TabIndex = 7;
            label5.Text = "📀";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 528);
            Controls.Add(trackBarDuration);
            Controls.Add(AlbumArtpictureBox);
            Controls.Add(Timelabel);
            Controls.Add(Circlelabel);
            Controls.Add(Yearlabel);
            Controls.Add(label5);
            Controls.Add(Albumlabel);
            Controls.Add(label4);
            Controls.Add(Artistlabel);
            Controls.Add(label3);
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
        public Label label3;
        public Label label4;
        public Label label5;
    }
}
