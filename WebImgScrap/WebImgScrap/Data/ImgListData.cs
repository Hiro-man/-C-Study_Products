namespace WebImgScrap.Data
{
    /// <summary>
    /// 画像データのリストのデータクラス
    /// </summary>
    public class ImgListData
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 画像データのリスト
        /// </summary>
        public List<ImgData> DataList { get; set; } = new List<ImgData>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="title">画像データを取得したページのタイトル</param>
        /// <param name="datas">取得した画像データのリスト</param>
        public ImgListData(string title, List<ImgData> datas)
        {
            Title = Define.AdjustTitle(title);
            DataList = datas;
        }
    }
}
