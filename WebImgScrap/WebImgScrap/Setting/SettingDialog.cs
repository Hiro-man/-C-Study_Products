namespace WebImgScrap.Setting
{
    /// <summary>
    /// 設定ダイアログ
    /// </summary>
    public partial class SettingDialog : Form
    {
        /// <summary>
        /// 設定データ
        /// </summary>
        public Data.SettingData settingData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">MainFormが持っている設定データのオブジェクト</param>
        public SettingDialog(Data.SettingData data)
        {
            InitializeComponent();
               
            // 初期値設定
            this.Text = Properties.Resources.APP_FORM_NAME;
            this.settingData = data;

            // データを各コントロールにバインド
            txtAutoSaveFolda.DataBindings.Add(new Binding("Text", settingData, "Path", false, DataSourceUpdateMode.OnPropertyChanged));
            chkAutoSave.DataBindings.Add(new Binding("Checked", settingData, "IsAutoSave", false, DataSourceUpdateMode.OnPropertyChanged));
            chkRetry.DataBindings.Add(new Binding("Checked", settingData, "IsRetry", false, DataSourceUpdateMode.OnPropertyChanged));
            nudRetryCnt.DataBindings.Add(new Binding("Value", settingData, "RetryCount", false, DataSourceUpdateMode.OnPropertyChanged));
        }

        /// <summary>
        /// chkRetryによってレイアウト変更
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">イベント</param>
        private void chkRetry_CheckedChanged(object sender, EventArgs e)
        {
            int length = 30;

            if (chkRetry.Checked)
            {
                lblExplanationRetryCnd.Visible = true;
                nudRetryCnt.Visible = true;
                nudRetryCnt.Enabled = true;
                this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height + length);
                btnClose.Location = new System.Drawing.Point(btnClose.Location.X, btnClose.Location.Y + length);
            }
            else
            {
                lblExplanationRetryCnd.Visible = false;
                nudRetryCnt.Visible = false;
                nudRetryCnt.Enabled = false;
                this.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height - length);
                btnClose.Location = new System.Drawing.Point(btnClose.Location.X, btnClose.Location.Y - length);
            }
        }

        /// <summary>
        /// 画像を自動保存する際の保存先フォルダを選択する
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">イベント</param>
        private void btnSelectAutoSaveFolda_Click(object sender, EventArgs e)
        {
            //FolderBrowserDialogクラスのインスタンスを作成
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "画像の保存先を指定してください。";
                fbd.InitialDirectory = txtAutoSaveFolda.Text;

                //ダイアログを表示する
                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    txtAutoSaveFolda.Text = fbd.SelectedPath;
                }
            }
        }

        /// <summary>
        /// パスを書き換えた際，存在しないフォルダパスの場合に初期値に戻す
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">イベント</param>
        private void txtAutoSaveFolda_Validated(object sender, EventArgs e)
        {
            // 存在しないフォルダパスの場合
            if (!Directory.Exists(txtAutoSaveFolda.Text))
            {
                // 初期値に戻す
                txtAutoSaveFolda.Text = Define.MY_PICTURE_PATH;
            }
        }
    }
}
