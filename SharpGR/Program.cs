using System.Diagnostics;
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

            catch (COMException)
            {
                DialogResult result = MessageBox.Show("���z�����W�I���Đ��o���܂���ł���\n\n���z�����W�I�̍Đ��y�[�W�ɃA�N�Z�X���A���W�I������ɍĐ�����邩�m�F���܂����H\n�A�N�Z�X�o�����獶���̍Đ��{�^���������Ă��������B", "COMException���X���[����܂���", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

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