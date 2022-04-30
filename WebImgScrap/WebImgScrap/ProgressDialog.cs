using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebImgScrap
{
    /// <summary>
    /// プログレスダイアログ
    /// </summary>
    public partial class ProgressDialog : Form
    {
        /// <summary>
        /// プログレスバーの進捗値
        /// </summary>
        public int Progress
        {
            set
            {
                progressBar.Value = value;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">ダイアログに表示するテキスト</param>
        public ProgressDialog(string message)
        {
            InitializeComponent();
            
            // テキストの設定
            this.Text = Application.ProductName;
            this.lblMessage.Text = message;
        }
    }
}
