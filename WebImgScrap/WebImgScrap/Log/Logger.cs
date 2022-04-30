using System.Text;

namespace WebImgScrap.Log
{
    /// <summary>
    /// ログを作成するクラス
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// ログを書き込むストリーム
        /// </summary>
        private StreamWriter logWriter;

        /// <summary>
        /// ロッカー
        /// </summary>
        private static object locker = new object();

        /// <summary>
        /// ログのフォーマット
        /// </summary>
        private const string logFormat = "[{0}]{1}:{2}";

#pragma warning disable CS8618 // logWriterにnull値が入っていることへの警告を無視
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Logger()
        {

        }
#pragma warning restore CS8618

        /// <summary>
        /// ストリームの開始処理
        /// </summary>
        /// <returns>true:成功/false:失敗</returns>
        public bool Open()
        {
#pragma warning disable CS8600,CS8604 // null値代入および参照の警告を無視
            string baseDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            // 実行可能ファイルと同じディレクトリにLogフォルダを用意し，そこにログファイルを作成する
            baseDirectory = Path.Combine(baseDirectory, "Log");
#pragma warning restore CS8600,CS8604
            // フォルダが無ければ作成
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            // ログファイルパスの作成
            // ※日付ごとに作成する
            string path = Path.Combine(baseDirectory, $"{Application.ProductName}_{DateTime.Now.ToString("yyyyMMdd")}.log");

            // ストリームの生成
            try
            {
                logWriter = new StreamWriter(path, true, Encoding.UTF8);
            }
            catch (System.UnauthorizedAccessException)
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// クローズ処理
        /// </summary>
        public void Close()
        {
            // ストリームを閉じる
            logWriter.Close();
        }

        /// <summary>
        /// ログの書き込み処理
        /// </summary>
        /// <param name="message">ログに残す内容</param>
        /// <param name="type">ログ種別</param>
        /// <param name="ex">エラー※エラーログの場合に指定</param>
        public void Write(string message, Define.LogType type, Exception? ex = null)
        {
            // 排他制御（順番にログを書き込む）
            lock (locker)
            {
                // エラーログの場合，エラーメッセージも記述する
                if (type == Define.LogType.Error && ex != null)
                {
                    message += $":{ex}::{ex.Message}" + Environment.NewLine;
                    if (string.IsNullOrEmpty(ex.StackTrace) == false) message += ex.StackTrace;
                }

                // 書き込む内容をログのフォーマットに合わせる
                string text = string.Format(logFormat,
                    type,
                    DateTime.Now,
                    message
                    );

                // ストリームにログメッセージを書き込む
                logWriter.WriteLine(text);
                // ログファイルに書き込む
                logWriter.Flush();
            }
        }
    }
}
