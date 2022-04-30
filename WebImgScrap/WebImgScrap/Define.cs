using Microsoft.Win32;
using System.Text;

namespace WebImgScrap
{
    public static class Define
    {
        /// <summary>
        /// ユーザが設定しているPictureフォルダのパス
        /// </summary>
        public static readonly string MY_PICTURE_PATH = GetMyPicturePath();

        /// <summary>
        /// 画像の正規表現パターン（mp4も含む）
        /// </summary>
        public const string IMG_PTTERN = @".(jpg)|(png)|(gif)|(svg)|(mp4)";

        public enum LogType
        {
            /// <summary>
            /// インフォメーション
            /// </summary>
            Info,

            /// <summary>
            /// 警告
            /// </summary>
            Warning,

            /// <summary>
            /// エラー
            /// </summary>
            Error
        }

        #region メソッド

        /// <summary>
        /// ユーザが設定しているPictureフォルダのパスを取得する
        /// </summary>
        /// <returns>ユーザが設定しているPictureフォルダのパス</returns>
        private static string GetMyPicturePath()
        {
            // キーを読み取り専用で開く
            RegistryKey? regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", false);
            // キーが存在しないとき
            if (regkey == null) return string.Empty;

            // キーが存在する場合，キーの値を取得して返す
            string? path = regkey.GetValue("My Pictures") as string;

            return path != null? path: "";
        }

        /// <summary>
        /// ファイル名に使用できない文字を変換する
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <returns></returns>
        public static string AdjustTitle(string title)
        {
            StringBuilder text = new StringBuilder();

            // 1文字ずつ読み込んでチェックする
            foreach (char character in title)
            {
                if (JudgeInvalidChar(character))
                {
                    text.Append(character);
                }
                else
                {
                    // ファイル名に使用できない文字の場合は_に変換する
                    text.Append("_");
                }

                // 名前が長すぎるとエラーになるため，
                // 100文字超えた場合は，その時点での文字列を返す
                if(text.Length >= 100)
                {
                    break;
                }
            }

            // 1文字目が_の場合は1文字目を削除
            while (true)
            {
                if (text[0].Equals("_"))
                {
                    text.Remove(0, 1);
                }
                else
                {
                    break;
                }
            }

            return text.ToString().TrimEnd();
        }

        /// <summary>
        /// ファイル名に使用できない文字かどうか判定する
        /// </summary>
        /// <param name="target">対象文字</param>
        /// <returns>true:使用可能/false:使用不可</returns>
        private static bool JudgeInvalidChar(char target)
        {
            // ファイル名に使用できない文字を取得
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();

            foreach (char invalidChar in invalidChars)
            {
                if (target.Equals(invalidChar))
                {
                    // ファイル名に使用できない文字の場合はfalse
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
