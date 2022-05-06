using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using WebImgScrap.Data;
using WebImgScrap.Setting.Data;

namespace WebImgScrap
{
    /// <summary>
    /// コントローラクラス（ダウンロード処理などの具体的な処理を実装）
    /// </summary>
    public class MainFormController
    {
        /// <summary>
        /// 取得した画像データ
        /// </summary>
        public List<ImgListData> imgListDatas = new List<ImgListData>();

        /// <summary>
        /// 取得に失敗した画像のURL
        /// </summary>
        private List<Url> failureImgUrl = new List<Url>();

        /// <summary>
        /// ブラウザみたいなもの？
        /// </summary>
        private IBrowsingContext browsingContext;

        /// <summary>
        /// ロガー
        /// </summary>
        private WebImgScrap.Log.Logger logger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="logger">ロガー</param>
        public MainFormController(WebImgScrap.Log.Logger logger)
        {
            // ロガーの設定
            this.logger = logger;

            // browsingContextの設定
            IConfiguration config = Configuration.Default.WithDefaultLoader().WithCss();
            
            // browsingContextの初期化
            this.browsingContext = BrowsingContext.New(config);
        }

        #region Webスクレイピング

        /// <summary>
        /// 指定したWebページをスクレイピングして画像データを取得する
        /// </summary>
        /// <param name="url">指定したWebページのURL</param>
        /// <param name="settingData">GUIで設定した設定データ</param>
        /// <param name="cancellationToken">非同期処理のキャンセル用のトークン</param>
        /// <returns>true:処理成功/false:処理失敗</returns>
        public async Task<bool> WebScraping(string url, SettingData settingData, CancellationToken cancellationToken)
        {
            // 処理結果
            bool res;

            // （前回呼び出し時で）取得した画像データをクリア
            imgListDatas.Clear();

            // 自動で保存するかどうかのフラグ
            bool isAutoSave = settingData.IsAutoSave;

            // ダウンロードしたいWebページに埋め込まれた画像のURLのリスト
            List<Url> urls = new List<Url>();

            // Webページを取得
            try
            {
                IDocument document = await this.browsingContext.OpenAsync(url);

                // Webページへのアクセスに失敗した場合，エラーメッセージを表示して，falseを返す
                if (document.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    //TODO:エラー処理
                    string note = $"HTTP Status Code {(int)document.StatusCode} : ダウンロードに失敗しました．";
                    logger.Write(note, Define.LogType.Error);
                    MessageBox.Show(note, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                // Webページのタイトルを取得←画像を保存するフォルダ名に使用する
                string title;
                string? dummyTitle = document.Title;
                if (string.IsNullOrEmpty(dummyTitle) != true)
                {
                    title = dummyTitle;
                }
                else
                {
                    title = "タイトルなし";
                }

                // imgタグから画像のURLを取得する
                foreach (IHtmlImageElement element in document.Images)
                {
                    Url imgUrl = new Url(element.Source);
                    urls.Add(imgUrl);
                }
                // 全タグを取得し，順にURLを調査
                IHtmlAllCollection allElements = document.All;
                foreach (var allElement in allElements)
                {
                    IHtmlElement? element = allElement as IHtmlElement;
                    if (element == null)
                    {
                        // IHtmlElementにキャストできなければスルーする
                        continue;
                    }

                    // imgタグからは取得済みなのでスキップする
                    if (element.TagName.Equals("IMG"))
                    {
                        continue;
                    }

                    // imgタグでない場合，属性をチェックして，画像のURL取得を試みる
                    INamedNodeMap attrs = element.Attributes;
                    foreach (IAttr attr in attrs)
                    {
                        if (Regex.IsMatch(attr.Value, Define.IMG_PTTERN))
                        {
                            Url imgUrl = new Url(attr.Value);
                            if (imgUrl.Href.Contains("http") == false && attr.BaseUrl != null)
                            {
                                imgUrl = new Url(attr.BaseUrl.Origin + attr.Value);
                            }
                            // 対象のファイルの拡張子が画像（mp4なども対象にしている）でない場合，URLのリストに追加しない
                            string? ext = Path.GetExtension(imgUrl.Href);
                            if (string.IsNullOrEmpty(ext) == true || ext.Equals(".cgi") == true)
                            {
                                continue;
                            }
                            // フルのURLが取得できた場合は取得対象のURLのリストに追加
                            if (imgUrl.PathName.Equals("/") == false)
                            {
                                urls.Add(imgUrl);
                            }
                        }
                    }
                }

                // ローダーの設定（FetchAsyncでbrowsingContext.OpenAsyncで取得したWebページにリクエストを送る．．．ハズ）
                IDocumentLoader loader;
                IDocumentLoader? dummyLoader = this.browsingContext.GetService<IDocumentLoader>();
                if (dummyLoader != null)
                {
                    loader = dummyLoader;
                }
                else
                {
                    loader = new DefaultDocumentLoader(this.browsingContext);
                }
                // Webページへのリクエストのヘッダーを設定
                DefaultHttpRequester requester;
                DefaultHttpRequester? dummyRequester = browsingContext.GetService<DefaultHttpRequester>();
                if (dummyRequester != null)
                {
                    requester = dummyRequester;
                }
                else
                {
                    requester = new DefaultHttpRequester();
                }
                requester.Headers["Accept"] = "text/html, application/xhtml+xml, image/jxr, */*";
                 
                // 処理結果（trueならエラーなし）
                res = await GetImgParallel(loader, title, urls, cancellationToken, isAutoSave, settingData.Path);

                // ダウンロードに失敗した画像がある場合，再ダウンロード
                // ただし，リトライする設定になっている場合のみ実行する．
                if (settingData.IsRetry == true && res == false)
                {
                    // 繰り返し回数
                    int cnt = 0;
                    // 処理に失敗したものが無くなる，または，リトライ回数を超えるまで繰り返す
                    do
                    {
                        Thread.Sleep(1000);

                        List<Url> dummyList = new List<Url>(failureImgUrl);
                        failureImgUrl.Clear();

                        res = await GetImgParallel(loader, title, dummyList, cancellationToken, isAutoSave, settingData.Path);
                        cnt++;
                    }
                    while (res == false && cnt < settingData.RetryCount);
                    failureImgUrl.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                // GUIで処理がキャンセルされた場合，MainForm（呼び出し元）にエラーを投げる
                throw;
            }
            // ネットの接続が上手くいかなかった場合
            catch (Exception ex)           
            {                
                logger.Write("", Define.LogType.Error, ex);
                res = false;
            }
            // 処理結果（成功/失敗）を返す
            return res;
        }

        /// <summary>
        /// 非同期に画像をダウンロードする
        /// </summary>
        /// <param name="loader">ローダー</param>
        /// <param name="title">タイトル（画像を格納するフォルダ名に使用）</param>
        /// <param name="urls">取得したい画像のURLのリスト</param>
        /// <param name="cancellationToken">非同期処理のキャンセル用のトークン</param>
        /// <param name="isAutoSave">自動で保存するかどうかのフラグ</param>
        /// <param name="baseSaveDirectory">画像を格納するフォルダのベースとなるフォルダパス</param>
        /// <returns>true:ダウンロード成功/false:ダウンロード失敗</returns>
        private async Task<bool> GetImgParallel(IDocumentLoader loader, string title, List<Url> urls, CancellationToken cancellationToken, bool isAutoSave = false, string baseSaveDirectory = "")
        {
            // 取得したデータのリスト
            List<ImgData> res = new List<ImgData>();
            // 取得に失敗したデータのリスト
            List<ImgData> failureRes = new List<ImgData>();

            try
            {
                logger.Write($"ダウンロードの開始:{title}：全{urls.Count}件", Define.LogType.Info);
                // 並列処理の設定
                ParallelOptions option = new ParallelOptions();
                option.MaxDegreeOfParallelism = 5;
                option.CancellationToken = cancellationToken;
                // 並列アクセス
                await Parallel.ForEachAsync(urls, option, async (url, cancell) =>
                {
                    // GUIの中止ボタンが押下された場合は処理を中止する
                    if(cancell.IsCancellationRequested)
                    {
                        cancell.ThrowIfCancellationRequested();
                    }
                    // 進捗ダイアログを作成（ダウンロード処理を進捗ダイアログのbackgroundWorkerに渡す）
#pragma warning disable CS8622 // new DoWorkEventHandler(ProgressDialog_DoWork)でのnull許容の警告を無視
                    ProgressDialog dialog = new ProgressDialog(
                        $"ダウンロード中:{url}",
                        new DoWorkEventHandler(ProgressDialog_DoWork),
                        loader,
                        url,
                        cancell
                        );
#pragma warning restore CS8622
                    // 進捗ダイアログを表示
                    DialogResult result = dialog.ShowDialog();                    
                    // 処理結果を取得
                    if (result == DialogResult.OK && dialog.Result != null)
                    {
                        // 取得する画像データを格納するbyte型配列
                        byte[] data = (byte[])dialog.Result;
                        // 取得した-データのサイズが0でなければ，画像の取得に成功
                        if (data.Length > 0)
                        {
                            // データをリストに登録
                            ImgData imgData = new ImgData(url.PathName, data);
                            res.Add(imgData);

                            if (isAutoSave)
                            {
                                string saveFoldaPath = CreateSaveFolda(baseSaveDirectory, Define.AdjustTitle(title));
                                SaveImgFile(saveFoldaPath, imgData);
                            }

                            logger.Write($"ダウンロードに成功:{url}", Define.LogType.Info);
                        }
                        else
                        {
                            //TODO:エラー処理
                            logger.Write($"ダウンロードに失敗:{url}", Define.LogType.Error);
                            failureImgUrl.Add(url);
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        throw new OperationCanceledException("A task was canceled.");
                    }
                    else
                    {
                        //TODO:エラー処理
                        int statusCode = 0;
                        try
                        {
                            int value = (int)dialog.ErrorValue;
                        }
                        catch (System.InvalidCastException)
                        {
                            // キャストに失敗した場合，statusCodeは0のまま使用する
                        }
                        string message = dialog.ErrorMessage;
                        if (dialog.Error != null)
                        {
                            logger.Write(message, Define.LogType.Error, dialog.Error);
                        }
                        else
                        {
                            logger.Write(message, Define.LogType.Error);
                        }

                        if (statusCode >= 400 && statusCode < 500)
                        {
                            /*
                             * 400番台はクライアント側のエラー→どうやってもダウンロード不可
                             * そのため，失敗リストにURLを追加しない
                             */
                        }
                        else
                        {
                            failureImgUrl.Add(url);
                        }
                    }
                    // 進捗ダイアログの後始末
                    dialog.Dispose();
                });
            }
            catch (OperationCanceledException)
            {
                // GUIで処理がキャンセルされた場合，MainForm（呼び出し元）にエラーを投げる
                throw;
            }
            catch (Exception ex)
            {
                //TODO:エラー処理
                logger.Write("", Define.LogType.Error, ex);
            }

            this.imgListDatas.Add(new ImgListData(title, res));

            // ダウンロードに失敗したデータがあった場合
            if(failureImgUrl.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 画像をダウンロードする（進捗ダイアログのbackgroundWorkerで処理を実行する）
        /// </summary>
        /// <param name="sender">イベントが発生したコントロール（BackgroundWorkerEXクラスのコントロール）</param>
        /// <param name="e">発生したDoWorkイベント</param>
        private void ProgressDialog_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorkerEX bw = (BackgroundWorkerEX)sender;

            // ローダー
            IDocumentLoader loader = (IDocumentLoader)bw.item1;
            // 取得対象の画像のURL
            Url url = (Url)bw.item2;
            // MainFormからのキャンセルトークン
            CancellationToken cancell = (CancellationToken)bw.item3;

            // 取得対象の画像のURLにアクセスし，アクセス結果を取得
            IResponse response = loader.FetchAsync(new DocumentRequest(url)).Task.Result;
            // アクセス結果を処理
            if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // 取得したコンテンツ
                Stream content = response.Content;

                // 取得する画像データを格納するbyte型配列
                byte[] data = new byte[] { };
                // 取得したコンテンツのデータサイズ
                int contentLength;
                //  取得したコンテンツのデータサイズ（以下のTryGetValueで使用するためのダミー）
                string? dummyLen;

                // Content-Lengthの有無で画像の取得方法を変更する
                if (response.Headers.TryGetValue("Content-Length", out dummyLen) == false)
                {
                    bw.ChangeProgressBarStyle(ProgressBarStyle.Marquee, 200);
                    bw.ReportProgress(0, "処理中．．．");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // GUIの中止ボタンが押下された場合は処理を中止する
                        if (cancell.IsCancellationRequested)
                        {
                            e.Cancel = true;
                            return;
                        }
                        // ダウンロード
                        try
                        {
                            content.CopyTo(ms);
                            data = ms.ToArray();
                        }
                        catch (System.Net.Sockets.SocketException ex)
                        {
                            bw.error = ex;
                            bw.errorMessage = "接続に失敗（接続済みの呼び出し先，または，接続済みのホストが応答しなかったため）";
                            return;
                        }
                        catch (System.IO.IOException ex)
                        {
                            bw.error = ex;
                            return;
                        }
                    }
                }
                else if (int.TryParse(dummyLen, out contentLength) == true)
                {
                    data = new byte[contentLength]; // response.Content.Lengthは使えないみたい

                    int index = 0;
                    int chunk;

                    while (true)
                    {
                        // GUIの中止ボタンが押下された場合は処理を中止する
                        if (cancell.IsCancellationRequested)
                        {
                            e.Cancel = true;
                            return;
                        }
                        // ダウンロード
                        try
                        {
                            chunk = content.ReadAsync(data, index, data.Length - index, cancell).Result;
                        }
                        catch (System.Net.Sockets.SocketException ex)
                        {
                            bw.error = ex;
                            bw.errorMessage = "接続に失敗（接続済みの呼び出し先，または，接続済みのホストが応答しなかったため）";
                            return;
                        }
                        catch (Exception ex)
                        {
                            bw.error = ex;
                            return;
                        }
                        if (chunk > 0)
                        {
                            index += chunk;
                            int percent = (index * 100) / contentLength;
                            bw.ReportProgress(percent, $"進捗率{percent}%");
                        }
                        else
                        {
                            // ダウンロード処理終了
                            break;
                        }
                    }
                }
                bw.ReportProgress(100, "finish");

                // 0.1秒スリープ
                Thread.Sleep(100);
                // 取得したデータをResultに設定する
                e.Result = data;                
            }
            else
            {
                int statusCode = 0;
                string message = $"通信に失敗:{url}";
                if (response != null)
                {
                    statusCode = (int)response.StatusCode;
                }
                bw.errorMessage = message;
                bw.errorValue = statusCode;
            } 
        }
        
        #endregion

        #region 画像の保存

        /// <summary>
        /// 画像を保存するフォルダを作成し，そのパスを返す
        /// </summary>
        /// <param name="basePath">保存先フォルダ</param>
        /// <param name="folda">フォルダ名</param>
        /// <returns>作成したフォルダパス</returns>
        public string CreateSaveFolda(string basePath, string folda)
        {
            // 画像データを取得したページのタイトルでフォルダのパスを作成
            string saveDirectoryPath = Path.Combine(basePath, folda);
            
            // フォルダが無ければ作成
            if (!Directory.Exists(saveDirectoryPath))
            {
                Directory.CreateDirectory(saveDirectoryPath);
            }

            return saveDirectoryPath;
        }

        /// <summary>
        /// 取得した画像データを保存する
        /// </summary>
        /// <param name="path">画像ファイルの保存先</param>
        /// <param name="imgData">画像データ</param>
        /// <returns>true:成功/false:失敗</returns>
        public bool SaveImgFile(string path, ImgData imgData)
        {
            try
            {
                string filePath = Path.Combine(path, imgData.Name);

                byte[] data = imgData.Data;

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    fs.Write(data);
                }
            }
            catch (Exception ex)
            {
                //TODO:エラー処理
                logger.Write("", Define.LogType.Error, ex);
                return false;
            }
            return true;
        }

        #endregion

    }
}
