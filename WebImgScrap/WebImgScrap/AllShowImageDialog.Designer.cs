namespace WebImgScrap
{
    partial class AllShowImageDialog
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
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.mnuItemAllSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tabCtrlShowImage = new System.Windows.Forms.TabControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.mainMenu.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemAllSave,
            this.mnuItemSave});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(589, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // mnuItemAllSave
            // 
            this.mnuItemAllSave.Name = "mnuItemAllSave";
            this.mnuItemAllSave.Size = new System.Drawing.Size(64, 20);
            this.mnuItemAllSave.Text = "全て保存";
            this.mnuItemAllSave.Click += new System.EventHandler(this.mnuItemAllSave_Click);
            // 
            // mnuItemSave
            // 
            this.mnuItemSave.Name = "mnuItemSave";
            this.mnuItemSave.Size = new System.Drawing.Size(122, 20);
            this.mnuItemSave.Text = "表示中の画像を保存";
            this.mnuItemSave.Click += new System.EventHandler(this.mnuItemSave_Click);
            // 
            // tabCtrlShowImage
            // 
            this.tabCtrlShowImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrlShowImage.Location = new System.Drawing.Point(3, 38);
            this.tabCtrlShowImage.Name = "tabCtrlShowImage";
            this.tabCtrlShowImage.SelectedIndex = 0;
            this.tabCtrlShowImage.Size = new System.Drawing.Size(583, 431);
            this.tabCtrlShowImage.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tabCtrlShowImage, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(589, 472);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.Controls.Add(this.lblFormTitle, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnRight, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnLeft, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(583, 29);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.AutoSize = true;
            this.lblFormTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFormTitle.Font = new System.Drawing.Font("たぬき油性マジック", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFormTitle.Location = new System.Drawing.Point(83, 0);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(417, 29);
            this.lblFormTitle.TabIndex = 5;
            this.lblFormTitle.Text = "画像一覧";
            this.lblFormTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRight
            // 
            this.btnRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRight.Font = new System.Drawing.Font("たぬき油性マジック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnRight.Location = new System.Drawing.Point(506, 3);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(74, 23);
            this.btnRight.TabIndex = 1;
            this.btnRight.Text = "次の画像";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLeft.Enabled = false;
            this.btnLeft.Font = new System.Drawing.Font("たぬき油性マジック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLeft.Location = new System.Drawing.Point(3, 3);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(74, 23);
            this.btnLeft.TabIndex = 0;
            this.btnLeft.Text = "前の画像";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // AllShowImageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 496);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "AllShowImageDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AllShowImageDialog";
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip mainMenu;
        private ToolStripMenuItem mnuItemAllSave;
        private ToolStripMenuItem mnuItemSave;
        private TabControl tabCtrlShowImage;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label lblFormTitle;
        private Button btnRight;
        private Button btnLeft;
    }
}