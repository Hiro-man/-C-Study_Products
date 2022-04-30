using Microsoft.Win32;
using WebImgScrap.Log;

namespace WebImgScrap
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ���K�[�̃C���X�^���X��
            WebImgScrap.Log.Logger logger = new WebImgScrap.Log.Logger();
            // ���O�t�@�C�����J���i���s����ꍇ�͖{�A�v�����N�����Ȃ��j
            if (logger.Open() == false)
            {
                // �G���[���b�Z�[�W��\�����ďI������
                MessageBox.Show(Properties.Resources.ErrorMessage_NonFileAuthority, Properties.Resources.ErrorTitle_NonFileAuthority, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mutex��
            string mutexName = "";
            bool createdNew;

            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                if(createdNew == false)
                {
                    // ���d�N���֎~���b�Z�[�W��\�����ďI������
                    MessageBox.Show(Properties.Resources.ErrorMessage_NonFileAuthority, Properties.Resources.ErrorTitle_NonFileAuthority, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new MainForm(logger));
                mutex.ReleaseMutex();
            }
        }
    }
}