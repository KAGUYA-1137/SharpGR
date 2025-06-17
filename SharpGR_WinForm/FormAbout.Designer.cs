namespace SharpGR_WinForm;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
        label1 = new Label();
        tabControl1 = new TabControl();
        tabPage1 = new TabPage();
        textBox2 = new TextBox();
        label4 = new Label();
        tabPage2 = new TabPage();
        textBox1 = new TextBox();
        labelVersion = new Label();
        pictureBox1 = new PictureBox();
        tabControl1.SuspendLayout();
        tabPage1.SuspendLayout();
        tabPage2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        SuspendLayout();
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(210, 9);
        label1.Margin = new Padding(4, 0, 4, 0);
        label1.Name = "label1";
        label1.Size = new Size(52, 15);
        label1.TabIndex = 0;
        label1.Text = "SharpGR";
        // 
        // tabControl1
        // 
        tabControl1.Controls.Add(tabPage1);
        tabControl1.Controls.Add(tabPage2);
        tabControl1.Location = new Point(21, 112);
        tabControl1.Margin = new Padding(4, 2, 4, 2);
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
        tabPage1.Margin = new Padding(4, 2, 4, 2);
        tabPage1.Name = "tabPage1";
        tabPage1.Padding = new Padding(4, 2, 4, 2);
        tabPage1.Size = new Size(433, 181);
        tabPage1.TabIndex = 0;
        tabPage1.Text = "情報";
        // 
        // textBox2
        // 
        textBox2.BorderStyle = BorderStyle.None;
        textBox2.Dock = DockStyle.Fill;
        textBox2.Location = new Point(4, 2);
        textBox2.Margin = new Padding(4, 2, 4, 2);
        textBox2.Multiline = true;
        textBox2.Name = "textBox2";
        textBox2.Size = new Size(425, 177);
        textBox2.TabIndex = 2;
        textBox2.Text = "幻想郷ラジオを再生するアプリケーションです。\r\n\r\n©KAGUYA_1137";
        textBox2.TextAlign = HorizontalAlignment.Center;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(172, 122);
        label4.Margin = new Padding(4, 0, 4, 0);
        label4.Name = "label4";
        label4.Size = new Size(0, 15);
        label4.TabIndex = 1;
        // 
        // tabPage2
        // 
        tabPage2.Controls.Add(textBox1);
        tabPage2.Location = new Point(4, 22);
        tabPage2.Margin = new Padding(4, 2, 4, 2);
        tabPage2.Name = "tabPage2";
        tabPage2.Padding = new Padding(4, 2, 4, 2);
        tabPage2.Size = new Size(433, 183);
        tabPage2.TabIndex = 1;
        tabPage2.Text = "クレジット";
        tabPage2.UseVisualStyleBackColor = true;
        // 
        // textBox1
        // 
        textBox1.BackColor = SystemColors.ControlLightLight;
        textBox1.BorderStyle = BorderStyle.None;
        textBox1.Font = new Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 128);
        textBox1.Location = new Point(0, 0);
        textBox1.Margin = new Padding(4, 2, 4, 2);
        textBox1.Multiline = true;
        textBox1.Name = "textBox1";
        textBox1.ReadOnly = true;
        textBox1.ScrollBars = ScrollBars.Both;
        textBox1.Size = new Size(433, 185);
        textBox1.TabIndex = 0;
        textBox1.TabStop = false;
        textBox1.Text = resources.GetString("textBox1.Text");
        textBox1.TextAlign = HorizontalAlignment.Center;
        // 
        // labelVersion
        // 
        labelVersion.AutoSize = true;
        labelVersion.Location = new Point(218, 92);
        labelVersion.Margin = new Padding(4, 0, 4, 0);
        labelVersion.Name = "labelVersion";
        labelVersion.Size = new Size(38, 15);
        labelVersion.TabIndex = 0;
        labelVersion.Text = "label2";
        // 
        // pictureBox1
        // 
        pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
        pictureBox1.Location = new Point(206, 28);
        pictureBox1.Margin = new Padding(4, 2, 4, 2);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(63, 62);
        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox1.TabIndex = 2;
        pictureBox1.TabStop = false;
        // 
        // FormAbout
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(479, 339);
        Controls.Add(pictureBox1);
        Controls.Add(tabControl1);
        Controls.Add(labelVersion);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        KeyPreview = true;
        Margin = new Padding(4, 2, 4, 2);
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
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    private PictureBox pictureBox1;
}