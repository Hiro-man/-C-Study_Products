using WebImgScrap.Data;

namespace WebImgScrap
{
    /// <summary>
    /// 取得した画像を表示するダイアログ
    /// </summary>
    public partial class AllShowImageDialog : Form
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private WebImgScrap.Log.Logger logger;

        /// <summary>
        /// 取得した画像を表示するダイアログ
        /// </summary>
        /// <param name="logger">ロガー</param>
        /// <param name="imgListDatas">画像データのリスト</param>
        public AllShowImageDialog(WebImgScrap.Log.Logger logger, List<ImgListData> imgListDatas)
        {
            InitializeComponent();

            // ロガーを呼び出し元からコピー
            this.logger = logger;
            // 順に画像をタブページに設定し，タブに追加
            foreach (ImgListData imgListData in imgListDatas)
            {
                if (imgListData != null)
                {
                    foreach (ImgData imgData in imgListData.DataList)
                    {
                        SetImage(imgListData.Title, imgData);
                    }
                }
            }
        }

        /// <summary>
        /// 画像をタブページのPictuteBoxに設定し，タブページをタブに追加する
        /// </summary>
        /// <param name="title">画像のリストのタイトル</param>
        /// <param name="imgData">画像データ</param>
        private void SetImage(string title, ImgData imgData)
        {
            // 画像を表示するPictuteBoxの定義
            PictureBox pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            // PictuteBoxを載せるタブの定義
            TabPage tabPage = new TabPage($"{title}/{imgData.Name}");
            tabPage.Controls.Add(pictureBox);

            try
            {
                // PictuteBoxに画像を表示する
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(imgData.Data);
                    pictureBox.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                logger.Write($"{imgData.Name}:画像の設定に失敗", Define.LogType.Error, ex);
                return;
            }

            // タブに追加
            this.tabCtrlShowImage.TabPages.Add(tabPage);
        }

        /// <summary>
        /// 本ダイアログで表示中の画像をすべて保存する
        /// </summary>
        private void mnuItemAllSave_Click(object sender, EventArgs e)
        {
            // ベースとなる保存先
            string saveDirectoryPath;

            // FolderBrowserDialogクラスのインスタンスを作成
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "画像の保存先を指定してください。";
                fbd.InitialDirectory = Define.MY_PICTURE_PATH;

                //ダイアログを表示する
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    saveDirectoryPath = fbd.SelectedPath;
                }
                else
                {
                    MessageBox.Show("保存処理を中止します", "保存中止", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            // タブページを順に読み込む
            foreach(TabPage tabPage in this.tabCtrlShowImage.TabPages)
            {
                // タブページのタイトルをファイル名にする
                string filepath = Path.Combine(saveDirectoryPath, tabPage.Text.Split('/')[1]);
                // 画像を保存する
                SaveImgFile(filepath, tabPage);
            }

        }

        /// <summary>
        /// 表示中の画像を保存する
        /// </summary>
        private void mnuItemSave_Click(object sender, EventArgs e)
        {
            // 選択中のタブを取得
            TabPage tabPage = this.tabCtrlShowImage.SelectedTab;
            // タブページのタイトルをデフォルトのファイル名に設定する
            string fileName = tabPage.Text.Split('/')[1];
            // デフォルトのファイル名より，拡張子を設定
            string ext = Path.GetExtension(fileName);
            // 保存ダイアログを開き，ファイルパスを取得する
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "画像ファイルの保存";
                saveFileDialog.CheckFileExists = false;
                saveFileDialog.CheckPathExists = false;
                saveFileDialog.InitialDirectory = Define.MY_PICTURE_PATH;
                saveFileDialog.FileName = fileName;
                saveFileDialog.Filter = $"{ext} file |*{ext}|All file |*.*";

                if(saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    // 指定したファイルパスで画像を保存する
                    SaveImgFile(saveFileDialog.FileName, tabPage);
                }
            }
        }

        /// <summary>
        /// 画像を保存する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <param name="tabPage">対象の画像を保持するタブページ</param>
        public void SaveImgFile(string path, TabPage tabPage)
        {
            // タブページから保存したい画像を表示しているPictuteBoxを取得
            PictureBox? picture = tabPage.Controls[0] as PictureBox;
            // PictuteBoxから画像データを取得する
            Image? img = picture?.Image;

            // PictuteBoxや画像データがない場合は処理しない
            if(picture == null || img == null)
            {
                logger.Write($"{path}:ファイルの保存に失敗:データがありません", Define.LogType.Error);
                return;
            }

            try
            {
                // 指定のファイルパスで画像を保存
                img.Save(path);
            }
            catch (Exception ex)
            {
                logger.Write($"{path}:ファイルの保存に失敗", Define.LogType.Error, ex);
            }
        }

        /// <summary>
        /// 次の画像を表示する（タブ選択を右に1つ進める）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRight_Click(object sender, EventArgs e)
        {
            int newIndex = this.tabCtrlShowImage.SelectedIndex + 1;
            if (newIndex >= this.tabCtrlShowImage.TabPages.Count - 1)
            {
                newIndex = this.tabCtrlShowImage.TabPages.Count - 1;
                // これ以上進められないので，押下不可にする
                this.btnRight.Enabled = false;
            }
            this.tabCtrlShowImage.SelectedIndex = newIndex;
            this.btnLeft.Enabled = true;
        }

        /// <summary>
        /// 前の画像を表示する（タブ選択を左に1つ進める）
        /// </summary>
        private void btnLeft_Click(object sender, EventArgs e)
        {
            int newIndex = tabCtrlShowImage.SelectedIndex - 1;
            if (newIndex < 0)
            {
                newIndex = 0;
                // これ以上進められないので，押下不可にする
                this.btnLeft.Enabled = false;
            }
            this.tabCtrlShowImage.SelectedIndex = newIndex;
            this.btnRight.Enabled= true;
        }
    }
}
