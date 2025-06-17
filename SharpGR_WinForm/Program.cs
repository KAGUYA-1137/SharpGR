namespace SharpGR_WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  アプリケーションのメインエントリポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                Application.Run(new Form1());
            }

            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}