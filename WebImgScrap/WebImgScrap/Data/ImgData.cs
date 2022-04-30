namespace WebImgScrap.Data
{
    /// <summary>
    /// 画像データのデータクラス
    /// </summary>
    public class ImgData
    {
        /// <summary>
        /// 画像名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 画像データ
        /// </summary>
        public byte[] Data { get; set; } = new byte[0];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">画像名</param>
        /// <param name="data">画像データ</param>
        public ImgData(string name, byte[] data)
        {
            Name = Define.AdjustTitle(name);
            Data = data;
        }
    }
}
