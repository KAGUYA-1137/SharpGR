using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using NAudio.Wave;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SharpGR_WPF
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void AppStartup(object sender, StartupEventArgs e)
        {
            try
            {
                string STREAM_ENDPOINT = "https://stream.gensokyoradio.net/3";
                using MediaFoundationReader mediaFoundationReader = new MediaFoundationReader(STREAM_ENDPOINT);

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                if (MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "起動に失敗しました。再試行しますか？", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop) == DialogResult.Yes)
                {
                    Current.Shutdown();
                    _ = Process.Start(ResourceAssembly.Location);
                }

                else
                {
                    Current.Shutdown();
                }
            }
        }
    }
}