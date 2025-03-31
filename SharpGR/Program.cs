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
            catch (COMException ex)
            {
                DialogResult result = MessageBox.Show("再生できませんでした。\n幻想郷ラジオのサーバーがダウンしている可能性があります。\n再試行しますか？", "COMExceptionがスローされました", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                {
                    Main();
                }
            }
        }
    }
}