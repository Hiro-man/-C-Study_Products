namespace WebImgScrap.Setting
{
    partial class SettingDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkRetry = new System.Windows.Forms.CheckBox();
            this.txtAutoSaveFolda = new System.Windows.Forms.TextBox();
            this.lblExplanationSaveFolda = new System.Windows.Forms.Label();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.nudRetryCnt = new System.Windows.Forms.NumericUpDown();
            this.lblExplanationRetryCnd = new System.Windows.Forms.Label();
            this.btnSelectAutoSaveFolda = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryCnt)).BeginInit();
            this.SuspendLayout();
            // 
            // chkRetry
            // 
            this.chkRetry.AutoSize = true;
            this.chkRetry.Checked = true;
            this.chkRetry.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRetry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkRetry.Location = new System.Drawing.Point(13, 164);
            this.chkRetry.Name = "chkRetry";
            this.chkRetry.Size = new System.Drawing.Size(278, 19);
            this.chkRetry.TabIndex = 19;
            this.chkRetry.Text = "ダウンロードに失敗した場合に再度ダウンロードを試みる";
            this.chkRetry.UseVisualStyleBackColor = true;
            this.chkRetry.CheckedChanged += new System.EventHandler(this.chkRetry_CheckedChanged);
            // 
            // txtAutoSaveFolda
            // 
            this.txtAutoSaveFolda.BackColor = System.Drawing.SystemColors.Window;
            this.txtAutoSaveFolda.Location = new System.Drawing.Point(12, 81);
            this.txtAutoSaveFolda.Name = "txtAutoSaveFolda";
            this.txtAutoSaveFolda.Size = new System.Drawing.Size(420, 23);
            this.txtAutoSaveFolda.TabIndex = 18;
            this.txtAutoSaveFolda.Validated += new System.EventHandler(this.txtAutoSaveFolda_Validated);
            // 
            // lblExplanationSaveFolda
            // 
            this.lblExplanationSaveFolda.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblExplanationSaveFolda.Location = new System.Drawing.Point(12, 41);
            this.lblExplanationSaveFolda.Name = "lblExplanationSaveFolda";
            this.lblExplanationSaveFolda.Size = new System.Drawing.Size(427, 49);
            this.lblExplanationSaveFolda.TabIndex = 17;
            this.lblExplanationSaveFolda.Text = "画像を自動保存する際の保存先を以下のテキストボックスに入力してください．\r\nもしくは，テキストボックス下のボタンからフォルダを選択してください．\r\n";
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Checked = true;
            this.chkAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkAutoSave.Location = new System.Drawing.Point(13, 139);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(205, 19);
            this.chkAutoSave.TabIndex = 15;
            this.chkAutoSave.Text = "ダウンロードした画像を自動で保存する";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("たぬき油性マジック", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(85, 19);
            this.lblTitle.TabIndex = 20;
            this.lblTitle.Text = "設定画面";
            // 
            // nudRetryCnt
            // 
            this.nudRetryCnt.Location = new System.Drawing.Point(242, 189);
            this.nudRetryCnt.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudRetryCnt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRetryCnt.Name = "nudRetryCnt";
            this.nudRetryCnt.Size = new System.Drawing.Size(49, 23);
            this.nudRetryCnt.TabIndex = 21;
            this.nudRetryCnt.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // lblExplanationRetryCnd
            // 
            this.lblExplanationRetryCnd.AutoSize = true;
            this.lblExplanationRetryCnd.Location = new System.Drawing.Point(28, 192);
            this.lblExplanationRetryCnd.Name = "lblExplanationRetryCnd";
            this.lblExplanationRetryCnd.Size = new System.Drawing.Size(208, 15);
            this.lblExplanationRetryCnd.TabIndex = 22;
            this.lblExplanationRetryCnd.Text = "→リトライ回数を指定します（※最大20）";
            // 
            // btnSelectAutoSaveFolda
            // 
            this.btnSelectAutoSaveFolda.Location = new System.Drawing.Point(12, 110);
            this.btnSelectAutoSaveFolda.Name = "btnSelectAutoSaveFolda";
            this.btnSelectAutoSaveFolda.Size = new System.Drawing.Size(124, 23);
            this.btnSelectAutoSaveFolda.TabIndex = 23;
            this.btnSelectAutoSaveFolda.Text = "フォルダを選択する";
            this.btnSelectAutoSaveFolda.UseVisualStyleBackColor = true;
            this.btnSelectAutoSaveFolda.Click += new System.EventHandler(this.btnSelectAutoSaveFolda_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(130, 227);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(198, 23);
            this.btnClose.TabIndex = 24;
            this.btnClose.Text = "設定画面を閉じる";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // SettingDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 258);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSelectAutoSaveFolda);
            this.Controls.Add(this.lblExplanationRetryCnd);
            this.Controls.Add(this.nudRetryCnt);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.chkRetry);
            this.Controls.Add(this.txtAutoSaveFolda);
            this.Controls.Add(this.lblExplanationSaveFolda);
            this.Controls.Add(this.chkAutoSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingDialog";
            ((System.ComponentModel.ISupportInitialize)(this.nudRetryCnt)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox chkRetry;
        private TextBox txtAutoSaveFolda;
        private Label lblExplanationSaveFolda;
        private CheckBox chkAutoSave;
        private Label lblTitle;
        private NumericUpDown nudRetryCnt;
        private Label lblExplanationRetryCnd;
        private Button btnSelectAutoSaveFolda;
        private Button btnClose;
    }
}