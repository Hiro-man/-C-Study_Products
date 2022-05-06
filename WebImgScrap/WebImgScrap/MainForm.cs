namespace WebImgScrap
{
    /// <summary>
    /// メイン画面
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// ダウンロードの設定データ
        /// </summary>
        Setting.Data.SettingData settingData = new Setting.Data.SettingData();

        /// <summary>
        /// コントローラ
        /// </summary>
        MainFormController controller;

        /// <summary>
        /// ロガー
        /// </summary>
        WebImgScrap.Log.Logger logger;

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
        /// <param name="logger">ロガー</param>
        public MainForm(WebImgScrap.Log.Logger logger)
        {
            InitializeComponent();

            // フォームタイトルを設定
            this.Text = Properties.Resources.APP_FORM_NAME;
            // ロガーの設定（Program.csでロガークラスをインスタンス化したものを代入する）
            this.logger = logger;
            // コントローラーの設定
            this.controller = new MainFormController(logger);
        }

        /// <summary>
        /// chkUrlMultiボタン押下時の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private void chkUrlMulti_CheckedChanged(object sender, EventArgs e)
        {
            // 変動幅（txtUrlの単一入力時の高さ×2）
            int length = 23 * 2;
            // 複数URLを指定する場合
            if (this.chkUrlMulti.Checked)
            {
                this.txtUrl.Multiline = true;
                this.txtUrl.ScrollBars = ScrollBars.Vertical;
                this.txtUrl.Size = new Size(txtUrl.Width, length + 23);
                // 各コントロールの位置をlength分だけ舌に移動させる
                this.chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y + length);
                this.txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y + length);
                this.lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y + length);
                this.Size = new Size(this.Size.Width, this.Size.Height + length);
            }
            // 単一のURL入力とする場合
            else 
            {
                this.txtUrl.Multiline= false;
                this.txtUrl.ScrollBars = ScrollBars.None;
                // 各コントロールの位置をlength分だけ上に移動させる
                this.chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y - length);
                this.txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y - length);
                this.lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y - length);
                this.Size = new Size(this.Size.Width, this.Size.Height - length);
            }
        }

        /// <summary>
        /// 開始ボタン押下時の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private async void btnStart_Click(object sender, EventArgs e)
        {
            // メッセージをクリア
            this.txtMessage.Text = "";

            // フラグを有効化
            this.isProcessing = true;

            // 一時的にボタンを無効化
            if(this.btnStart.Enabled)
            {
                this.btnStart.Enabled = false;
                this.btnClear.Enabled = false;
                this.chkUrlMulti.Enabled = false;
                this.txtUrl.ReadOnly = true;

            }
            this.btnCancel.Enabled = true;

            // GUIで入力されたURLを文字列配列として取得
            string[] urls = this.txtUrl.Text.Split("\n").Select(url => url.Trim()).ToArray();
            WriteInfo($"全{urls.Length}件のダウンロードを開始");

            // ダウンロード処理の成功/失敗のフラグ
            bool flag = true;
            // 処理した件数のカウント
            int cnt = 1;

            // 入力したURLを順に処理する
            foreach (string target in urls)
            {
                // URL文字列
                string url = target;
                // URL文字列が正常かどうか判定
                if (Define.JusgeURL(ref url) == false)
                {
                    WriteInfo($"入力された文字列がURLとして不正です．:{url}");
                    this.txtMessage.AppendText(":スキップします．");
                    continue;
                }
                // 「https://~.html#a 」のようなURLの場合，「#a」部分を削除する
                else if (string.Compare(url, target) == -1)
                {
                    WriteInfo($"入力された文字列を修正しました．{Environment.NewLine}　{target}{Environment.NewLine}→{url}");
                }
                
                try
                {
                    if (string.IsNullOrEmpty(url) == false)
                    {
                        WriteInfo($"{cnt}件目：{url}からのダウンロードを開始");
                        // ダウンロード処理を実行
                        flag = await controller.WebScraping(url, settingData, cts.Token) ? flag : false;
                        if (flag == false)
                        {
                            WriteInfo($"{cnt}件目：ダウンロード処理に失敗しました．");
                            this.txtMessage.AppendText("詳しくはログファイルをご確認ください．");
                        }
                        // 1秒待機
                        Thread.Sleep(1000);
                        WriteInfo($"{cnt}件目：{url}からのダウンロード処理終了");
                        // 件数をカウントアップ
                        cnt++;
                    } 
                }
                // 処理中にキャンセルボタンが押された場合
                catch (OperationCanceledException)
                {
                    // ログとGUIに出力
                    WriteInfo("ユーザによってダウンロード処理を中止", Define.LogType.Warning);
                    // ctsをクリア
                    this.cts = new CancellationTokenSource();
                    // ループ処理を中断する
                    break;
                }
            }

            // フラグをクリア
            this.isProcessing = false;

            // ボタンの無効化を解除
            if (btnStart.Enabled == false)
            {
                this.btnStart.Enabled = true;
                this.btnClear.Enabled = true;
                this.chkUrlMulti.Enabled = true;
                this.txtUrl.ReadOnly = false;
            }
            this.btnCancel.Enabled = false;

            // 処理が終了したことをユーザに通知
            WriteInfo("ダウンロード処理を終了");
        }

        /// <summary>
        /// テキストボックスとログにメッセージを出力する
        /// </summary>
        /// <param name="text">対象のテキスト</param>
        /// <param name="type">ログの種類</param>
        private void WriteInfo(string text, Define.LogType type = Define.LogType.Info)
        {
            this.txtMessage.AppendText(Environment.NewLine + $"[{DateTime.Now}]{text}");
            this.logger.Write(text, type);            
        }

        /// <summary>
        /// クローズ処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">FormClosingイベント</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ロガーのクローズ
            this.logger.Close();
        }

        /// <summary>
        /// 設定メニュークリック時の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private void mnuItemSetting_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("ダウンロード処理中は設定できません．", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // 設定ダイアログを表示
                using (Setting.SettingDialog settingDialog = new Setting.SettingDialog(this.settingData))
                {
                    settingDialog.ShowDialog(this);
                    this.settingData = settingDialog.settingData;
                }
            }
        }

        /// <summary>
        /// 画像一覧メニュークリック時の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private void mnuItemImageShow_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("ダウンロード処理中は表示できません．", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                List<Data.ImgListData> data = controller.imgListDatas;

                if (data.Count > 0)
                {
                    // 取得した画像を表示するダイアログを表示
                    using (AllShowImageDialog imageDialog = new AllShowImageDialog(this.logger, data))
                    {
                        imageDialog.ShowDialog(this);
                    }
                }
                else
                {
                    MessageBox.Show("表示する画像がありません．", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// ダウンロード処理を中止する
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cts.Cancel();
            // 複数回押下できないように，1度押下したら再度押下できないようにする
            this.btnCancel.Enabled = false;
        }

        /// <summary>
        /// URLを入力するテキストボックスを空にする
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">発生したイベント</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtUrl.Text = "";
        }
    }
}