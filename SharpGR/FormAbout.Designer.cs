namespace SharpGR
{
    partial class FormAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
            label1 = new Label();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            textBox2 = new TextBox();
            label4 = new Label();
            tabPage2 = new TabPage();
            linkLabelZUN = new LinkLabel();
            linkLabelGensokyo = new LinkLabel();
            textBox1 = new TextBox();
            labelVersion = new Label();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(222, 9);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 0;
            label1.Text = "SharpGR";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(21, 113);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(441, 209);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.ControlLightLight;
            tabPage1.Controls.Add(textBox2);
            tabPage1.Controls.Add(label4);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(433, 181);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "情報";
            // 
            // textBox2
            // 
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Dock = DockStyle.Fill;
            textBox2.Location = new Point(3, 3);
            textBox2.Multiline = true;
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(427, 175);
            textBox2.TabIndex = 2;
            textBox2.Text = "幻想郷ラジオを再生するアプリケーションです。\r\n\r\n©KAGUYA_1137";
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(172, 123);
            label4.Name = "label4";
            label4.Size = new Size(0, 15);
            label4.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(linkLabelZUN);
            tabPage2.Controls.Add(linkLabelGensokyo);
            tabPage2.Controls.Add(textBox1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(433, 181);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "クレジット";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // linkLabelZUN
            // 
            linkLabelZUN.AutoSize = true;
            linkLabelZUN.Location = new Point(136, 154);
            linkLabelZUN.Name = "linkLabelZUN";
            linkLabelZUN.Size = new Size(169, 15);
            linkLabelZUN.TabIndex = 2;
            linkLabelZUN.TabStop = true;
            linkLabelZUN.Text = "https://www16.big.or.jp/~zun/";
            linkLabelZUN.LinkClicked += linkLabelZUN_LinkClicked;
            // 
            // linkLabelGensokyo
            // 
            linkLabelGensokyo.AutoSize = true;
            linkLabelGensokyo.Location = new Point(136, 108);
            linkLabelGensokyo.Name = "linkLabelGensokyo";
            linkLabelGensokyo.Size = new Size(150, 15);
            linkLabelGensokyo.TabIndex = 2;
            linkLabelGensokyo.TabStop = true;
            linkLabelGensokyo.Text = "https://gensokyoradio.net/";
            linkLabelGensokyo.LinkClicked += linkLabelGensokyo_LinkClicked;
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ControlLightLight;
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
            textBox1.Location = new Point(0, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(433, 185);
            textBox1.TabIndex = 0;
            textBox1.TabStop = false;
            textBox1.Text = "SharpGRの開発者\r\nKAGUYA_1137\r\n\r\n\r\n\r\n謝辞\r\nLunarSpotlight Media(Gensokyo Radio)\r\n\r\n\r\n上海アリス幻樂団 / ZUN(東方Project)\r\n";
            textBox1.TextAlign = HorizontalAlignment.Center;
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Location = new Point(222, 49);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(38, 15);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "label2";
            // 
            // FormAbout
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(480, 339);
            Controls.Add(tabControl1);
            Controls.Add(labelVersion);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormAbout";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormAbout";
            Load += FormAbout_Load;
            KeyDown += FormAbout_KeyDown;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label4;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label labelVersion;
        private LinkLabel linkLabelGensokyo;
        private LinkLabel linkLabelZUN;
    }
}