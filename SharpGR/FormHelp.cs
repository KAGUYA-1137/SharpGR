using System.Reflection;

namespace SharpGR
{
    public partial class FormHelp : Form
    {
        public FormHelp()
        {
            InitializeComponent();
        }

        private void FormHelp_Load(object sender, EventArgs e)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Text = $"{assembly.GetName().Name}　操作方法";
            }

            catch
            {
                Form1 form1 = new Form1();
                form1.toolStripStatusLabel1.ForeColor = Color.Red;
                form1.toolStripStatusLabel1.Text = $"{Text}画面の表示中にエラーが発生しました";
            }
        }

        private void FormHelp_KeyDown(object sender, KeyEventArgs e)
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
