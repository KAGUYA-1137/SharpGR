using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpGR
{
    internal static class Program
    {
        /// <summary>
        ///  アプリケーションのメインエントリポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            try
            {
                Application.Run(new Form1());
            }

            catch (COMException)
            {
                DialogResult result = MessageBox.Show("幻想郷ラジオを再生出来ませんでした\n\n幻想郷ラジオの再生ページにアクセスし、ラジオが正常に再生されるか確認しますか？\nアクセス出来たら左下の再生ボタンを押してください。", "COMExceptionがスローされました", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = "https://gensokyoradio.net/playing/",
                        UseShellExecute = true
                    };
                    _ = Process.Start(startInfo);
                }
            }
        }
    }
}