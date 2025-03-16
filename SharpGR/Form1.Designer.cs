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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonPlay = new Button();
            trackBarVol = new TrackBar();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            ヘルプToolStripMenuItem = new ToolStripMenuItem();
            操作方法ToolStripMenuItem = new ToolStripMenuItem();
            sharpGRについてToolStripMenuItem = new ToolStripMenuItem();
            numericUpDownVol = new NumericUpDown();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)trackBarVol).BeginInit();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDownVol).BeginInit();
            SuspendLayout();
            // 
            // buttonPlay
            // 
            buttonPlay.Location = new Point(339, 95);
            buttonPlay.Name = "buttonPlay";
            buttonPlay.Size = new Size(81, 35);
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
            trackBarVol.Size = new Size(268, 45);
            trackBarVol.TabIndex = 1;
            trackBarVol.Scroll += trackBarVol_Scroll;
            trackBarVol.ValueChanged += trackBarVol_ValueChanged;
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
            menuStrip1.Size = new Size(440, 24);
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
            numericUpDownVol.Location = new Point(339, 37);
            numericUpDownVol.Name = "numericUpDownVol";
            numericUpDownVol.Size = new Size(58, 23);
            numericUpDownVol.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(403, 41);
            label2.Name = "label2";
            label2.Size = new Size(17, 15);
            label2.TabIndex = 2;
            label2.Text = "%";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(440, 142);
            Controls.Add(numericUpDownVol);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(trackBarVol);
            Controls.Add(buttonPlay);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
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
    }
}
