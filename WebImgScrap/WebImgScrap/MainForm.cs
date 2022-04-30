namespace WebImgScrap
{
    public partial class MainForm : Form
    {

        /// <summary>
        /// 
        /// </summary>
        private string Message 
        {
            set
            {
                txtMessage.AppendText(Environment.NewLine + value);
            }
        }

        /// <summary>
        /// ダウンロードの設定データ
        /// </summary>
        Setting.Data.SettingData settingData = new Setting.Data.SettingData();

        /// <summary>
        /// コントローラ
        /// </summary>
        MainFormController controller = null;

        /// <summary>
        /// ロガー
        /// </summary>
        WebImgScrap.Log.Logger logger = null;

        /// <summary>
        /// ダウンロード処理のキャンセル判定
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// ダウンロード処理実行中かどうかのフラグ
        /// </summary>
        private bool isProcessing = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm(WebImgScrap.Log.Logger logger)
        {
            InitializeComponent();

            //
            this.logger = logger;

            //
            controller = new MainFormController(logger);

            // デフォルトの画像の保存先を表示
            //txtAutoSaveFolda.Text = saveDirectory;
        }

        private void chkUrlMulti_CheckedChanged(object sender, EventArgs e)
        {
            //
            int length = 23 * 2;

            if (chkUrlMulti.Checked)
            {
                txtUrl.Multiline = true;
                txtUrl.ScrollBars = ScrollBars.Vertical;
                txtUrl.Size = new Size(txtUrl.Width, length + 23);

                chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y + length);
                txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y + length);
                lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y + length);
                this.Size = new Size(this.Size.Width, this.Size.Height + length);
            }
            else 
            {
                txtUrl.Multiline= false;
                txtUrl.ScrollBars = ScrollBars.None;

                chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y - length);
                txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y - length);
                lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y - length);
                this.Size = new Size(this.Size.Width, this.Size.Height - length);
            }
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            //
            txtMessage.Text = "";

            //
            this.isProcessing = true;

            // 一時的に無効化
            if(btnStart.Enabled)
            {
                btnStart.Enabled = false;
                btnClear.Enabled = false;
                chkUrlMulti.Enabled = false;
                txtUrl.ReadOnly = true;

            }
            btnCancel.Enabled = true;

            //
            string[] urls = txtUrl.Text.Split("\n").Select(url => url.Trim()).ToArray();
            WriteInfo($"全{urls.Length}件のダウンロードを開始");

            //
            bool flag = true;

            //
            int cnt = 1;

            //
            foreach (string url in urls)
            {
                WriteInfo($"{cnt}件目：{url}からのダウンロードを開始");

                try
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        flag = await controller.WebScraping(url, settingData, cts.Token) ? flag : false;
                        Thread.Sleep(1000);
                    }

                    WriteInfo($"{cnt}件目：{url}ダウンロード処理終了");
                    

                    cnt++;
                }
                catch (OperationCanceledException calcellEx)
                {
                    //
                    cts = new CancellationTokenSource();
                    //
                    break;
                }
            }

            //
            this.isProcessing = false;

            // 無効化を解除
            if (btnStart.Enabled == false)
            {
                btnStart.Enabled = true;
                btnClear.Enabled = true;
                chkUrlMulti.Enabled = true;
                txtUrl.ReadOnly = false;
            }
            btnCancel.Enabled = false;

            // 
            WriteInfo("ダウンロード処理を終了");
        }

        /// <summary>
        /// テキストボックスとログにメッセージを出力する
        /// </summary>
        /// <param name="text"></param>
        private void WriteInfo(string text)
        {
            Message = $"[{DateTime.Now}]{text}";
            logger.Write(text, Define.LogType.Info);
        }

        /// <summary>
        /// クローズ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ロガーのクローズ
            logger.Close();
        }

        private void mnuItemSetting_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("ダウンロード処理中は設定できません．", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                using (Setting.SettingDialog settingDialog = new Setting.SettingDialog(this.Text, this.settingData))
                {
                    settingDialog.ShowDialog(this);
                    settingData = settingDialog.settingData;
                }
            }
        }

        private void mnuItemImageShow_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("ダウンロード処理中は表示できません．", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                List<Data.ImgListData> data = controller.imgListDatas;

                if (data.Count > 0)
                {
                    using (AllShowImageDialog imageDialog = new AllShowImageDialog(this.logger, data))
                    {
                        imageDialog.ShowDialog(this);
                    }
                }
                else
                {
                    MessageBox.Show("表示する画像がありません．", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// ダウンロード処理を中止する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        /// <summary>
        /// URLを入力するテキストボックスを空にする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUrl.Text = "";
        }
    }
}