using System.Diagnostics;
using System.Reflection;

namespace SharpGR
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Text = $"{assembly.GetName().Name}について";
                labelVersion.Text = assembly.GetName().Version.ToString();
            }

            catch
            {
                Form1 form1 = new Form1();
                form1.toolStripStatusLabel1.ForeColor = Color.Red;
                form1.toolStripStatusLabel1.Text = $"{Text}画面の表示中にエラーが発生しました";
            }
        }

        private void linkLabelGensokyo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabelGensokyo.LinkVisited = true;

                Process.Start(new ProcessStartInfo
                {
                    FileName = linkLabelGensokyo.Text,
                    UseShellExecute = true
                });
            }

            catch
            {
                Form1 form1 = new Form1();
                form1.toolStripStatusLabel1.ForeColor = Color.Red;
                form1.toolStripStatusLabel1.Text = "幻想郷ラジオのリンクをクリックした時にエラーが発生しました";
            }
        }

        private void linkLabelZUN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabelZUN.LinkVisited = true;

                Process.Start(new ProcessStartInfo
                {
                    FileName = linkLabelZUN.Text,
                    UseShellExecute = true
                });
            }

            catch
            {
                Form1 form1 = new Form1();
                form1.toolStripStatusLabel1.ForeColor = Color.Red;
                form1.toolStripStatusLabel1.Text = "上海アリス幻樂団のリンクをクリックした時にエラーが発生しました";
            }
        }

        private void FormAbout_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }
    }
}
