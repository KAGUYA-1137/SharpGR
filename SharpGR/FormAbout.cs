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
