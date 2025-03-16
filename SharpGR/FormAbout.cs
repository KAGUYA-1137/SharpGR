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
            Assembly assembly = Assembly.GetExecutingAssembly();
            Text = $"{assembly.GetName().Name}について";
            labelVersion.Text = assembly.GetName().Version.ToString();
        }

        private void linkLabelGensokyo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelGensokyo.LinkVisited = true;
            Process.Start(new ProcessStartInfo
            {
                FileName = linkLabelGensokyo.Text,
                UseShellExecute = true
            });
        }

        private void linkLabelZUN_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelZUN.LinkVisited = true;
            Process.Start(new ProcessStartInfo
            {
                FileName = linkLabelZUN.Text,
                UseShellExecute = true
            });
        }
    }
}
