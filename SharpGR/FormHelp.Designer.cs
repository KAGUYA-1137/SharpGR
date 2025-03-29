namespace SharpGR
{
    partial class FormHelp
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
            label2 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 18);
            label1.Name = "label1";
            label1.Size = new Size(245, 15);
            label1.TabIndex = 0;
            label1.Text = "Escキーを押すとその時表示していた画面を閉じます";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(23, 52);
            label2.Name = "label2";
            label2.Size = new Size(184, 15);
            label2.TabIndex = 0;
            label2.Text = "音量スライダーで音量を調整出来ます";
            // 
            // FormHelp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(320, 145);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormHelp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormHelp";
            Load += FormHelp_Load;
            KeyDown += FormHelp_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
    }
}