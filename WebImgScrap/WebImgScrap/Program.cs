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
            // ロガーのインスタンス化
            WebImgScrap.Log.Logger logger = new WebImgScrap.Log.Logger();
            // ログファイルを開く（失敗する場合は本アプリを起動しない）
            if (logger.Open() == false)
            {
                // エラーメッセージを表示して終了する
                MessageBox.Show(Properties.Resources.ErrorMessage_NonFileAuthority, Properties.Resources.ErrorTitle_NonFileAuthority, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mutex名
            string mutexName = "";
            bool createdNew;

            using (Mutex mutex = new Mutex(true, mutexName, out createdNew))
            {
                if(createdNew == false)
                {
                    // 多重起動禁止メッセージを表示して終了する
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