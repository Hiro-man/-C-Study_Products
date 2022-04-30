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
        /// �_�E�����[�h�̐ݒ�f�[�^
        /// </summary>
        Setting.Data.SettingData settingData = new Setting.Data.SettingData();

        /// <summary>
        /// �R���g���[��
        /// </summary>
        MainFormController controller = null;

        /// <summary>
        /// ���K�[
        /// </summary>
        WebImgScrap.Log.Logger logger = null;

        /// <summary>
        /// �_�E�����[�h�����̃L�����Z������
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// �_�E�����[�h�������s�����ǂ����̃t���O
        /// </summary>
        private bool isProcessing = false;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MainForm(WebImgScrap.Log.Logger logger)
        {
            InitializeComponent();

            //
            this.logger = logger;

            //
            controller = new MainFormController(logger);

            // �f�t�H���g�̉摜�̕ۑ����\��
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

            // �ꎞ�I�ɖ�����
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
            WriteInfo($"�S{urls.Length}���̃_�E�����[�h���J�n");

            //
            bool flag = true;

            //
            int cnt = 1;

            //
            foreach (string url in urls)
            {
                WriteInfo($"{cnt}���ځF{url}����̃_�E�����[�h���J�n");

                try
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        flag = await controller.WebScraping(url, settingData, cts.Token) ? flag : false;
                        Thread.Sleep(1000);
                    }

                    WriteInfo($"{cnt}���ځF{url}�_�E�����[�h�����I��");
                    

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

            // ������������
            if (btnStart.Enabled == false)
            {
                btnStart.Enabled = true;
                btnClear.Enabled = true;
                chkUrlMulti.Enabled = true;
                txtUrl.ReadOnly = false;
            }
            btnCancel.Enabled = false;

            // 
            WriteInfo("�_�E�����[�h�������I��");
        }

        /// <summary>
        /// �e�L�X�g�{�b�N�X�ƃ��O�Ƀ��b�Z�[�W���o�͂���
        /// </summary>
        /// <param name="text"></param>
        private void WriteInfo(string text)
        {
            Message = $"[{DateTime.Now}]{text}";
            logger.Write(text, Define.LogType.Info);
        }

        /// <summary>
        /// �N���[�Y����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���K�[�̃N���[�Y
            logger.Close();
        }

        private void mnuItemSetting_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("�_�E�����[�h�������͐ݒ�ł��܂���D", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                MessageBox.Show("�_�E�����[�h�������͕\���ł��܂���D", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    MessageBox.Show("�\������摜������܂���D", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// �_�E�����[�h�����𒆎~����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }

        /// <summary>
        /// URL����͂���e�L�X�g�{�b�N�X����ɂ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUrl.Text = "";
        }
    }
}