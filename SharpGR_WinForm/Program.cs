namespace SharpGR_WinForm
{
    internal static class Program
    {
        /// <summary>
        ///  �A�v���P�[�V�����̃��C���G���g���|�C���g�ł��B
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
                _ = MessageBox.Show(ex.Message, "�G���[���������܂����B", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}