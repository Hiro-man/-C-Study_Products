namespace WebImgScrap
{
    /// <summary>
    /// ���C�����
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// �_�E�����[�h�̐ݒ�f�[�^
        /// </summary>
        Setting.Data.SettingData settingData = new Setting.Data.SettingData();

        /// <summary>
        /// �R���g���[��
        /// </summary>
        MainFormController controller;

        /// <summary>
        /// ���K�[
        /// </summary>
        WebImgScrap.Log.Logger logger;

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
        /// <param name="logger">���K�[</param>
        public MainForm(WebImgScrap.Log.Logger logger)
        {
            InitializeComponent();

            // �t�H�[���^�C�g����ݒ�
            this.Text = Properties.Resources.APP_FORM_NAME;
            // ���K�[�̐ݒ�iProgram.cs�Ń��K�[�N���X���C���X�^���X���������̂�������j
            this.logger = logger;
            // �R���g���[���[�̐ݒ�
            this.controller = new MainFormController(logger);
        }

        /// <summary>
        /// chkUrlMulti�{�^���������̏���
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private void chkUrlMulti_CheckedChanged(object sender, EventArgs e)
        {
            // �ϓ����itxtUrl�̒P����͎��̍����~2�j
            int length = 23 * 2;
            // ����URL���w�肷��ꍇ
            if (this.chkUrlMulti.Checked)
            {
                this.txtUrl.Multiline = true;
                this.txtUrl.ScrollBars = ScrollBars.Vertical;
                this.txtUrl.Size = new Size(txtUrl.Width, length + 23);
                // �e�R���g���[���̈ʒu��length��������Ɉړ�������
                this.chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y + length);
                this.txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y + length);
                this.lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y + length);
                this.Size = new Size(this.Size.Width, this.Size.Height + length);
            }
            // �P���URL���͂Ƃ���ꍇ
            else 
            {
                this.txtUrl.Multiline= false;
                this.txtUrl.ScrollBars = ScrollBars.None;
                // �e�R���g���[���̈ʒu��length��������Ɉړ�������
                this.chkUrlMulti.Location = new Point(chkUrlMulti.Location.X, chkUrlMulti.Location.Y - length);
                this.txtMessage.Location = new Point(txtMessage.Location.X, txtMessage.Location.Y - length);
                this.lblTitle_txtMessage.Location = new Point(lblTitle_txtMessage.Location.X, lblTitle_txtMessage.Location.Y - length);
                this.Size = new Size(this.Size.Width, this.Size.Height - length);
            }
        }

        /// <summary>
        /// �J�n�{�^���������̏���
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private async void btnStart_Click(object sender, EventArgs e)
        {
            // ���b�Z�[�W���N���A
            this.txtMessage.Text = "";

            // �t���O��L����
            this.isProcessing = true;

            // �ꎞ�I�Ƀ{�^���𖳌���
            if(this.btnStart.Enabled)
            {
                this.btnStart.Enabled = false;
                this.btnClear.Enabled = false;
                this.chkUrlMulti.Enabled = false;
                this.txtUrl.ReadOnly = true;

            }
            this.btnCancel.Enabled = true;

            // GUI�œ��͂��ꂽURL�𕶎���z��Ƃ��Ď擾
            string[] urls = this.txtUrl.Text.Split("\n").Select(url => url.Trim()).ToArray();
            WriteInfo($"�S{urls.Length}���̃_�E�����[�h���J�n");

            // �_�E�����[�h�����̐���/���s�̃t���O
            bool flag = true;
            // �������������̃J�E���g
            int cnt = 1;

            // ���͂���URL�����ɏ�������
            foreach (string target in urls)
            {
                // URL������
                string url = target;
                // URL�����񂪐��킩�ǂ�������
                if (Define.JusgeURL(ref url) == false)
                {
                    WriteInfo($"���͂��ꂽ������URL�Ƃ��ĕs���ł��D:{url}");
                    this.txtMessage.AppendText(":�X�L�b�v���܂��D");
                    continue;
                }
                // �uhttps://~.html#a �v�̂悤��URL�̏ꍇ�C�u#a�v�������폜����
                else if (string.Compare(url, target) == -1)
                {
                    WriteInfo($"���͂��ꂽ��������C�����܂����D{Environment.NewLine}�@{target}{Environment.NewLine}��{url}");
                }
                
                try
                {
                    if (string.IsNullOrEmpty(url) == false)
                    {
                        WriteInfo($"{cnt}���ځF{url}����̃_�E�����[�h���J�n");
                        // �_�E�����[�h���������s
                        flag = await controller.WebScraping(url, settingData, cts.Token) ? flag : false;
                        if (flag == false)
                        {
                            WriteInfo($"{cnt}���ځF�_�E�����[�h�����Ɏ��s���܂����D");
                            this.txtMessage.AppendText("�ڂ����̓��O�t�@�C�������m�F���������D");
                        }
                        // 1�b�ҋ@
                        Thread.Sleep(1000);
                        WriteInfo($"{cnt}���ځF{url}����̃_�E�����[�h�����I��");
                        // �������J�E���g�A�b�v
                        cnt++;
                    } 
                }
                // �������ɃL�����Z���{�^���������ꂽ�ꍇ
                catch (OperationCanceledException)
                {
                    // ���O��GUI�ɏo��
                    WriteInfo("���[�U�ɂ���ă_�E�����[�h�����𒆎~", Define.LogType.Warning);
                    // cts���N���A
                    this.cts = new CancellationTokenSource();
                    // ���[�v�����𒆒f����
                    break;
                }
            }

            // �t���O���N���A
            this.isProcessing = false;

            // �{�^���̖�����������
            if (btnStart.Enabled == false)
            {
                this.btnStart.Enabled = true;
                this.btnClear.Enabled = true;
                this.chkUrlMulti.Enabled = true;
                this.txtUrl.ReadOnly = false;
            }
            this.btnCancel.Enabled = false;

            // �������I���������Ƃ����[�U�ɒʒm
            WriteInfo("�_�E�����[�h�������I��");
        }

        /// <summary>
        /// �e�L�X�g�{�b�N�X�ƃ��O�Ƀ��b�Z�[�W���o�͂���
        /// </summary>
        /// <param name="text">�Ώۂ̃e�L�X�g</param>
        /// <param name="type">���O�̎��</param>
        private void WriteInfo(string text, Define.LogType type = Define.LogType.Info)
        {
            this.txtMessage.AppendText(Environment.NewLine + $"[{DateTime.Now}]{text}");
            this.logger.Write(text, type);            
        }

        /// <summary>
        /// �N���[�Y����
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">FormClosing�C�x���g</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ���K�[�̃N���[�Y
            this.logger.Close();
        }

        /// <summary>
        /// �ݒ胁�j���[�N���b�N���̏���
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private void mnuItemSetting_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("�_�E�����[�h�������͐ݒ�ł��܂���D", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // �ݒ�_�C�A���O��\��
                using (Setting.SettingDialog settingDialog = new Setting.SettingDialog(this.settingData))
                {
                    settingDialog.ShowDialog(this);
                    this.settingData = settingDialog.settingData;
                }
            }
        }

        /// <summary>
        /// �摜�ꗗ���j���[�N���b�N���̏���
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private void mnuItemImageShow_Click(object sender, EventArgs e)
        {
            if (isProcessing)
            {
                MessageBox.Show("�_�E�����[�h�������͕\���ł��܂���D", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                List<Data.ImgListData> data = controller.imgListDatas;

                if (data.Count > 0)
                {
                    // �擾�����摜��\������_�C�A���O��\��
                    using (AllShowImageDialog imageDialog = new AllShowImageDialog(this.logger, data))
                    {
                        imageDialog.ShowDialog(this);
                    }
                }
                else
                {
                    MessageBox.Show("�\������摜������܂���D", Properties.Resources.APP_FORM_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        /// <summary>
        /// �_�E�����[�h�����𒆎~����
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.cts.Cancel();
            // �����񉟉��ł��Ȃ��悤�ɁC1�x����������ēx�����ł��Ȃ��悤�ɂ���
            this.btnCancel.Enabled = false;
        }

        /// <summary>
        /// URL����͂���e�L�X�g�{�b�N�X����ɂ���
        /// </summary>
        /// <param name="sender">�C�x���g�����������R���g���[��</param>
        /// <param name="e">���������C�x���g</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtUrl.Text = "";
        }
    }
}