namespace WebImgScrap
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.chkUrlMulti = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuItemSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemImageShow = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTitle_txtMessage = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtUrl
            // 
            resources.ApplyResources(this.txtUrl, "txtUrl");
            this.txtUrl.Name = "txtUrl";
            // 
            // chkUrlMulti
            // 
            resources.ApplyResources(this.chkUrlMulti, "chkUrlMulti");
            this.chkUrlMulti.Name = "chkUrlMulti";
            this.chkUrlMulti.UseVisualStyleBackColor = true;
            this.chkUrlMulti.CheckedChanged += new System.EventHandler(this.chkUrlMulti_CheckedChanged);
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblExplanation
            // 
            resources.ApplyResources(this.lblExplanation, "lblExplanation");
            this.lblExplanation.Name = "lblExplanation";
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.Black;
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.txtMessage, "txtMessage");
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            // 
            // mainMenu
            // 
            this.mainMenu.BackColor = System.Drawing.SystemColors.Control;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemSetting,
            this.mnuItemImageShow});
            resources.ApplyResources(this.mainMenu, "mainMenu");
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // mnuItemSetting
            // 
            this.mnuItemSetting.Name = "mnuItemSetting";
            resources.ApplyResources(this.mnuItemSetting, "mnuItemSetting");
            this.mnuItemSetting.Click += new System.EventHandler(this.mnuItemSetting_Click);
            // 
            // mnuItemImageShow
            // 
            this.mnuItemImageShow.Name = "mnuItemImageShow";
            resources.ApplyResources(this.mnuItemImageShow, "mnuItemImageShow");
            this.mnuItemImageShow.Click += new System.EventHandler(this.mnuItemImageShow_Click);
            // 
            // lblTitle_txtMessage
            // 
            resources.ApplyResources(this.lblTitle_txtMessage, "lblTitle_txtMessage");
            this.lblTitle_txtMessage.Name = "lblTitle_txtMessage";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClear
            // 
            resources.ApplyResources(this.btnClear, "btnClear");
            this.btnClear.Name = "btnClear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTitle_txtMessage);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lblExplanation);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.chkUrlMulti);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox txtUrl;
        private CheckBox chkUrlMulti;
        private Button btnStart;
        private Label lblExplanation;
        private TextBox txtMessage;
        private MenuStrip mainMenu;
        private ToolStripMenuItem mnuItemSetting;
        private ToolStripMenuItem mnuItemImageShow;
        private Label lblTitle_txtMessage;
        private Button btnCancel;
        private Button btnClear;
    }
}