using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// コントローラクラス（ダウンロード処理などを実装）
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

        /// <summary>
        /// 指定したWebページをスクレイピングして画像データを取得する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="settingData"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public async Task<bool> WebScraping(string url, SettingData settingData, CancellationToken cancellationToken, string basePath = "")
        {
            // （前回呼び出し時で）取得した画像データをクリア
            imgListDatas.Clear();

            // 自動で保存するかどうかのフラグ
            bool isAutoSave = settingData.IsAutoSave;

            // ダウンロードしたいWebページに埋め込まれた画像のURLのリスト
            List<Url> urls = new List<Url>();

            // Wepページを取得
            IDocument document = await this.browsingContext.OpenAsync(url);

            // Webページへのアクセスに失敗した場合，エラーメッセージを表示して，falseを返す
            if(document.StatusCode != System.Net.HttpStatusCode.OK)
            {
                //TODO

                string note = $"HTTP Status Code {(int)document.StatusCode} : ダウンロードに失敗しました．";
                logger.Write(note,Define.LogType.Error);
                MessageBox.Show(note, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }

            // Webページのタイトルを取得←画像を保存するフォルダ名に使用する
            string title = document.Title;

            // imgタグから画像のURLを取得する
            foreach (IHtmlImageElement element in document.Images)
            {
                Url imgUrl = new Url(element.Source);
                urls.Add(imgUrl);
            }

            IHtmlAllCollection allElements = document.All;
            foreach (IHtmlElement element in allElements)
            {
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
                        if (imgUrl.Href.Contains("http") == false)
                        {
                            imgUrl = new Url(attr.BaseUrl.Origin + attr.Value);
                        }
                        if (imgUrl.PathName.Equals("/") == true) continue;
                        urls.Add(imgUrl);
                    }
                }
            }
            
            IDocumentLoader loader = browsingContext.GetService<IDocumentLoader>();
            DefaultHttpRequester requester = browsingContext.GetService<DefaultHttpRequester>();
            requester.Headers["Accept"] = "text/html, application/xhtml+xml, image/jxr, */*";

            bool res = true;

            try
            {
                res = await GetImgParallel(loader, title, urls, cancellationToken, isAutoSave, settingData.Path);

                // ダウンロードに失敗した画像がある場合，再ダウンロード
                // ただし，リトライする設定になっている場合のみ実行する．
                if (settingData.IsRetry && res == false)
                {
                    // 繰り返し回数
                    int cnt = 0;

                    do
                    {
                        Thread.Sleep(1000);

                        List<Url> dummy = new List<Url>(failureImgUrl);
                        failureImgUrl.Clear();

                        res = await GetImgParallel(loader, title, dummy, cancellationToken, isAutoSave, settingData.Path);
                        cnt++;
                    }
                    while (res == false && cnt < settingData.RetryCount);
                    failureImgUrl.Clear();
                }
            }
            catch (OperationCanceledException calcellEx)
            {
                //
                throw calcellEx;
            }

            return res;
        }

        /// <summary>
        /// 非同期に画像をダウンロードする
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="title"></param>
        /// <param name="urls"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="isAutoSave"></param>
        /// <param name="baseSaveDirectory"></param>
        /// <returns></returns>
        private async Task<bool> GetImgParallel(IDocumentLoader loader, string title, List<Url> urls, CancellationToken cancellationToken, bool isAutoSave = false, string baseSaveDirectory = "")
        {
            List<ImgData> res = new List<ImgData>();
            List<ImgData> failureRes = new List<ImgData>();

            try
            {
                logger.Write($"ダウンロードの開始:{title}：全{urls.Count}件", Define.LogType.Info);

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

                    var response = await loader.FetchAsync(new DocumentRequest(url)).Task;

                    if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        try
                        {
                            using (ProgressDialog dialog = new ProgressDialog($"ダウンロード中:{url}"))
                            {
                                // 画像取得の進捗ダイアログを表示
                                dialog.ShowDialog();
                                //
                                Stream content = response.Content;

                                // 取得する画像データを格納するbyte型配列
                                byte[] data = new byte[] { };

                                //
                                int contentLength;
                                //
                                string dummyLen;
                                //
                                if (!response.Headers.TryGetValue("Content-Length", out dummyLen))
                                {
                                    using (MemoryStream ms = new MemoryStream())
                                    {
                                        try
                                        {
                                            content.CopyTo(ms);
                                            data = ms.ToArray();
                                            dialog.Progress = 100;
                                        }
                                        catch (System.IO.IOException ex)
                                        {
                                            //TODO:エラー処理
                                            logger.Write("", Define.LogType.Error, ex);
                                        }
                                    }

                                }
                                else if (int.TryParse(dummyLen, out contentLength))
                                {

                                    data = new byte[contentLength]; // response.Content.Lengthは使えないみたい

                                    int index = 0;
                                    int chunk = 0;

                                    while ((chunk = await content.ReadAsync(data, index, data.Length - index)) > 0)
                                    {
                                        index += chunk;

                                        dialog.Progress = (index * 100) / contentLength;
                                        logger.Write($"{index}/ {contentLength}:{index / contentLength}:{(index * 100) / contentLength}", Define.LogType.Info);
                                    }

                                }                            

                                // 0.1秒スリープ
                                Thread.Sleep(100);
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
                        }
                        catch (Exception ex)
                        {
                            //TODO:エラー処理
                            logger.Write("", Define.LogType.Error, ex);
                            Console.WriteLine(ex);
                        }

                    }
                    else
                    {
                        //TODO:エラー処理
                        int statusCode = 0;
                        string message = $"通信に失敗:{url}";
                        if(response != null)
                        {
                            statusCode = (int)response.StatusCode;
                            message += $"::HTTP Status Code {statusCode}";
                        }
                        logger.Write(message, Define.LogType.Error);

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
                });
            }
            catch (OperationCanceledException calcellEx)
            {
                // GUIで処理がキャンセルされた場合
                logger.Write("ユーザによってダウンロード処理を中止", Define.LogType.Warning, calcellEx);
                throw calcellEx;
            }
            catch (Exception ex)
            {
                //TODO:エラー処理
                logger.Write("", Define.LogType.Error, ex);
            }

            imgListDatas.Add(new ImgListData(title, res));

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
