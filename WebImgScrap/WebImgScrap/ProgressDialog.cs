using System.ComponentModel;

namespace WebImgScrap
{
    /// <summary>
    /// プログレスダイアログ
    /// </summary>
    public partial class ProgressDialog : Form
    {
        /// <summary>
        /// 処理結果（backgroundWorkerで処理して得たデータなど）
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// backgroundWorkerで処理中に発生したエラー
        /// </summary>
        public Exception? Error { get; set; } = null;

        /// <summary>
        /// backgroundWorkerで処理中に発生したエラーに対するエラーメッセージ
        /// </summary>
        public string ErrorMessage
        {
            get 
            {
                return this.backgroundWorker1.errorMessage;
            }
        }

        /// <summary>
        /// backgroundWorkerで処理中に発生したエラーに対する値（Webスクレイピング処理の場合はHTTPステータスコードなど）
        /// </summary>
        public object ErrorValue 
        { 
            get
            {
                return this.backgroundWorker1.errorValue;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="message">ダイアログに表示するテキスト</param>
        public ProgressDialog(string message, DoWorkEventHandler doWorkEvent, object? item1 = null, object? item2 = null, object? item3 = null)
        {
            InitializeComponent();
            
            // テキストの設定
            this.Text = Properties.Resources.APP_FORM_NAME;
            this.lblMessage.Text = message;
            // backgroundWorkerで実行する処理を設定
            this.backgroundWorker1.DoWork += doWorkEvent;
            // backgroundWorker実行時に使用するオブジェクトを設定
            if (item1 != null)
            {
                this.backgroundWorker1.item1 = item1;
            }
            if (item2 != null)
            {
                this.backgroundWorker1.item2 = item2;
            }
            if (item3 != null)
            {
                this.backgroundWorker1.item3 = item3;
            }
            // backgroundWorkerに本フォームの進捗バーを設定
            this.backgroundWorker1.progressBar = this.progressBar;
        }

        /// <summary>
        /// 本フォーム表示後の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">イベント</param>
        private void ProgressDialog_Shown(object sender, EventArgs e)
        {
            // backgroundWorkerの処理を開始
            this.backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// backgroundWorkerの処理で進捗が変更された際の処理
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">ProgressChangedEventイベント</param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int value = e.ProgressPercentage;
            // 進捗バーの最大値以上の場合
            if (value >= this.progressBar.Maximum)
            {
                value = this.progressBar.Maximum;
            }
            // 進捗バーの最小値以下の場合
            else if (value <= this.progressBar.Minimum)
            {
                value -= this.progressBar.Minimum;
            }
            // 進捗バーを更新
            this.progressBar.Value = value;
            // ステータス表示を更新
            object? state = e.UserState;
            if (state != null)
            {
                lblStatus.Text = state as string;
            }
        }

        /// <summary>
        /// backgroundWorkerの処理が完了した際の処理 
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール</param>
        /// <param name="e">RunWorkerCompletedイベント</param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                // 処理がキャンセルされた場合，処理結果を設定せず，DialogResult.Cancelをthis.DialogResultに設定する
                // ※処理がキャンセルされた状態でe.Resultを呼ぶとエラーが発生する
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                // backgroundWorkerでの処理結果を設定
                this.Result = e.Result;
                this.Error = this.backgroundWorker1.error;
                this.DialogResult = DialogResult.No;
                if (e.Error != null)
                {
                    this.Error = e.Error;
                }
                // エラーが無ければ，DialogResult.OKを返す
                else if (string.IsNullOrEmpty(this.backgroundWorker1.errorMessage) == true)
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
            // 本フォームを閉じる
            this.Close();
        }
    }

    /// <summary>
    /// 拡張backgroundWorker（別クラスで実行する処理を定義するために，使用するメンバを定義）
    /// </summary>
    class BackgroundWorkerEX : BackgroundWorker
    {
        /// <summary>
        /// backgroundWorkerで処理実行時に使用するオブジェクト その1
        /// </summary>
        public object item1 = new object();

        /// <summary>
        /// backgroundWorkerで処理実行時に使用するオブジェクト その2
        /// </summary>
        public object item2 = new object();

        /// <summary>
        /// backgroundWorkerで処理実行時に使用するオブジェクト その3
        /// </summary>
        public object item3 = new object();

        /// <summary>
        /// backgroundWorkerで処理実行時に発生したエラー
        /// </summary>
        public Exception? error = null;

        /// <summary>
        /// backgroundWorkerで処理実行時に発生したエラーに対するエラーメッセージ
        /// </summary>
        public string errorMessage = string.Empty;

        /// <summary>
        /// backgroundWorkerで処理実行時に発生したエラーに対する値
        /// </summary>
        public object errorValue = new object();

        /// <summary>
        /// プログレスバー
        /// </summary>
        public ProgressBar progressBar = new ProgressBar();

        /// <summary>
        /// プログレスバーのスタイルを変更する
        /// </summary>
        /// <param name="style">プログレスバーのスタイル</param>
        /// <param name="speed">マーキースタイル時の移動スピード</param>
        public void ChangeProgressBarStyle(ProgressBarStyle style, int speed = 100)
        {
            this.progressBar.Invoke(() => { 
                this.progressBar.Style = style;
            });
            this.progressBar.Invoke(() => {
                this.progressBar.MarqueeAnimationSpeed = speed;
            });
        }
    }
}
