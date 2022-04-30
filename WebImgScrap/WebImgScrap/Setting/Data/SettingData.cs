namespace WebImgScrap.Setting.Data
{
    /// <summary>
    /// 画像ダウンロードの設定データ
    /// </summary>
    public class SettingData
    {
        /// <summary>
        /// 保存先フォルダパス
        /// </summary>
        public string Path { get; set; } = Define.MY_PICTURE_PATH;

        /// <summary>
        /// 自動で画像を保存するかどうかのフラグ
        /// </summary>
        public bool IsAutoSave { get; set; } = true;

        /// <summary>
        /// ダウンロードに失敗した際，再度ダウンロードを試みるかどうかのフラグ
        /// </summary>
        public bool IsRetry { get; set; } = true;

        /// <summary>
        /// ダウンロードのリトライ回数
        /// </summary>
        public int RetryCount { get; set; } = 5;
    }
}
