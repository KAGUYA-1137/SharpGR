using System.Runtime.InteropServices;

namespace SharpGR
{
    internal static class Program
    {
        /// <summary>
        ///  �A�v���P�[�V�����̃��C���G���g���|�C���g�ł��B
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
                DialogResult result = MessageBox.Show("�Đ��ł��܂���ł����B\n���z�����W�I�̃T�[�o�[���_�E�����Ă���\��������܂��B\n�Ď��s���܂����H", "COMException���X���[����܂���", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                {
                    Main();
                }
            }
        }
    }
}