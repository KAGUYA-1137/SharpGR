using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using SharpGR.Commands;
using SharpGR.FileIO;
using SharpGR.Helper;
using SharpGR.Property;

namespace SharpGR.ViewModels
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region プロパティ

        /// <summary>
        /// タイトルバーに表示する文字列を取得または設定します
        /// </summary>
        public string AppTitle
        {
            get => appTitle;
            set => SetProperty(ref appTitle, value);
        }
        private string appTitle = string.Empty;

        /// <summary>
        /// 音量を取得または設定します
        /// </summary>
        public double Volume
        {
            get => volume;
            set
            {
                // プロパティの値を更新し、変更があった場合
                if (SetProperty(ref volume, value))
                {
                    // 音量を0.0から1.0の範囲に変換して音量を反映
                    waveOutEvent.Volume = (float)(volume / 100.0);
                }
            }
        }
        private double volume = 0.0;

        /// <summary>
        /// アルバムアートを取得または設定します
        /// </summary>
        public BitmapImage AlbumArt
        {
            get => albumArt;
            set => SetProperty(ref albumArt, value);
        }
        private BitmapImage albumArt = new BitmapImage();

        /// <summary>
        /// 楽曲名を取得または設定します
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        private string title = Constants.BLANK;

        /// <summary>
        /// アーティスト名を取得または設定します
        /// </summary>
        public string Artist
        {
            get => artist;
            set => SetProperty(ref artist, value);
        }
        private string artist = Constants.BLANK;

        /// <summary>
        /// アルバムIDを取得または設定します
        /// </summary>
        public string Album
        {
            get => album;
            set => SetProperty(ref album, value);
        }
        private string album = Constants.BLANK;

        /// <summary>
        /// 経過時間と総再生時間を取得または設定します
        /// </summary>
        public string Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }
        private string time = "--:--/--:--";

        /// <summary>
        /// 現在の楽曲の再生位置を表すスライダーの値を取得または設定します
        /// </summary>
        public double TimeSliderValue
        {
            get => timeSliderValue;
            set => SetProperty(ref timeSliderValue, value);
        }
        private double timeSliderValue = 0.0;

        /// <summary>
        /// 現在の楽曲の再生位置を表すスライダーの最大値を取得または設定します
        /// </summary>
        public double TimeSliderMaxValue
        {
            get => timeSliderMaxValue;
            set => SetProperty(ref timeSliderMaxValue, value);
        }
        private double timeSliderMaxValue = 0.0;

        /// <summary>
        /// 再生中かどうかを取得または設定します
        /// </summary>
        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }
        private bool isPlaying = false;

        #endregion

        #region フィールド

        /// <summary>
        /// アプリケーションの設定値管理
        /// </summary>
        private readonly SettingInfo _settingInfo = new SettingInfo();

        /// <summary>
        /// 再生の開始タスク
        /// </summary>
        private readonly Task startRadioTask;

        /// <summary>
        /// 再生の開始を非同期で行うためのキャンセレーショントークンソース
        /// </summary>
        private readonly CancellationTokenSource startRadioCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// オーディオストリームを読み取るためのインスタンスです
        /// <see cref="Constants.STREAM_ENDPOINT"/>で指定されたストリームエンドポイントからオーディオデータを読み取ります
        /// </summary>
        private readonly MediaFoundationReader _mediaFoundationReader = new MediaFoundationReader(Constants.STREAM_ENDPOINT);

        /// <summary>
        /// NAudioの出力デバイス管理
        public WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// HTTP リクエストを送信するための <see cref="HttpClient"/> の共有インスタンスです
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 幻想郷ラジオのAPIから取得した楽曲情報を格納するためのインスタンスです
        /// </summary>
        private RadioAPI radioAPI = new RadioAPI();

        /// <summary>
        /// アセンブリ名を格納するためのフィールドです
        /// </summary>
        private readonly AssemblyName appName = Assembly.GetExecutingAssembly().GetName();

        /// <summary>
        /// バージョン番号
        /// </summary>
        private readonly Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// 設定ファイルのパス
        /// </summary>
        private string filePath = string.Empty;

        #endregion

        #region コマンド

        /// <summary>
        /// 再生状態を変更するコマンドを取得します
        /// </summary>
        public DelegateCommand ChangePlaybackStateCommand { get; private set; }

        /// <summary>
        /// アルバム情報を表示するコマンドを取得します
        /// </summary>
        public DelegateCommand OpenAlbumInfoCommand { get; private set; }

        /// <summary>
        /// 設定を保存するコマンドを取得します
        /// </summary>
        public DelegateCommand SaveSettingCommand { get; private set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            try
            {
                // コマンドとメソッドの関連付け
                ChangePlaybackStateCommand = new DelegateCommand(ChangePlaybackState);
                OpenAlbumInfoCommand = new DelegateCommand(ClickedAlbumArt);
                SaveSettingCommand = new DelegateCommand(Shutdown);

                startRadioCancellationTokenSource = new CancellationTokenSource();

                // WaveOutEvent を MediaFoundationReader で初期化
                waveOutEvent.Init(_mediaFoundationReader);

                // アプリケーションのタイトルをアセンブリの名前とバージョンから構築
                AppTitle = $"{appName.Name} {appVersion.Major}.{appVersion.Minor}";

                filePath = Path.Combine(Constants.SETTING_FOOTER, Constants.SETTING_FILE_NAME);

                // 設定ファイルの保存先ディレクトリのパス
                var directoryName = Path.GetDirectoryName(filePath) ??
                                     throw new InvalidOperationException("無効なファイルパスです。");

                // 設定ファイルの保存先ディレクトリが存在しない場合
                if (!Directory.Exists(directoryName))
                {
                    // 設定ファイルの保存先ディレクトリを作成
                    Directory.CreateDirectory(directoryName);
                }

                // 設定ファイルが存在しない、または内容が空の場合
                if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
                {
                    // デフォルトの設定値を使用して設定ファイルを作成
                    JsonFileManager.SaveSetting(filePath, _settingInfo);
                }

                // 設定ファイルから読み込んだ設定値
                var settingInfo = JsonFileManager.LoadSetting(filePath);

                // 読み込んだ設定値をフィールドの settingInfo に反映
                _settingInfo = settingInfo;

                // 設定値をメイン画面に反映
                Volume = settingInfo.Volume;

                // 音量を0.0から1.0の範囲に変換して反映
                waveOutEvent.Volume = (float)(settingInfo.Volume / 100.0);

                // 再生の開始を非同期で行う
                startRadioTask = Task.Run(() => StartRadio(startRadioCancellationTokenSource.Token));
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                MessageBoxHelper.ShowExceptionMessageBox(exception);

                // 起動時の例外発生は致命的なため、アプリケーションを終了
                Application.Current.Shutdown(1);
            }
        }

        /// <summary>
        /// 再生を開始
        /// オーディオストリームの再生を開始し、楽曲情報の取得も非同期で実施
        /// </summary>
        /// <param name="cancellationToken">キャンセルトークンソース</param>
        private async Task StartRadio(CancellationToken cancellationToken)
        {
            try
            {
                // 再生の開始
                waveOutEvent.Play();

                IsPlaying = true;

                // HTTPリクエストの User-Agent ヘッダーにアプリケーションの名前とバージョンを追加
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{appName.Name} {appVersion.Major}.{appVersion.Minor} (https://github.com/KAGUYA-1137/SharpGR)");

                await GetSongInfoAsync(cancellationToken);
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 楽曲情報を非同期で取得し、プロパティに反映
        /// </summary>
        /// <returns><see cref="Task"/>オブジェクト</returns>
        private async Task GetSongInfoAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // HTTP GETリクエストを作成
                    using var request = new HttpRequestMessage(HttpMethod.Get, Constants.PLAYING_API_URL);

                    // 既にUser-Agentヘッダーが存在する
                    if (request.Headers.UserAgent.Count > 0)
                    {
                        // User-Agentヘッダーをクリア
                        request.Headers.UserAgent.Clear();
                    }

                    // HTTPリクエストのUser-Agentヘッダーにアプリケーションの名前とバージョンを追加
                    request.Headers.UserAgent.ParseAdd(httpClient.DefaultRequestHeaders.UserAgent.ToString());

                    // HTTPリクエストを楽曲情報の取得先に対して送信し、レスポンスを受け取る
                    using (var response = await GetSongInfoWithRetryAsync())
                    {
                        // 取得した JSON データを RadioAPI クラスのインスタンスに変換
                        radioAPI = JsonFileManager.ParseResponse(await response.Content.ReadAsStringAsync(cancellationToken));
                    }

                    // 変換した RadioAPI インスタンスが null ではない
                    if (radioAPI != null)
                    {
                        // アルバムアートのURL
                        var albumArtUrl = radioAPI.MISC.ALBUMART;

                        // アルバムアートの URL が空でない
                        if (!string.IsNullOrWhiteSpace(albumArtUrl))
                        {
                            // アルバムアートの URL を使用して画像を取得し、プロパティに反映
                            AlbumArt = await DownloadImageAsync($"{Constants.ALBUM_ART_PREFIX}{albumArtUrl}");
                        }
                        // アルバムアートの URL が空
                        else
                        {
                            // プレースホルダー画像をプロパティに反映
                            AlbumArt = await DownloadImageAsync(Constants.LOGO_PLACE_HOLDER_URL);
                        }

                        // 楽曲情報をプロパティに反映
                        Title = radioAPI.SONGINFO.TITLE;
                        Artist = radioAPI.SONGINFO.ARTIST;
                        Album = radioAPI.SONGINFO.ALBUM;

                        // アルバム名が「album」でない = インターミッションではない
                        if (radioAPI.SONGINFO.ALBUM != "album")
                        {
                            TimeSliderMaxValue = radioAPI.SONGTIMES.DURATION;

                            // 現在の再生時間が楽曲の総再生時間に5秒足した秒数になるまで繰り返し
                            // 5秒足すのは楽曲の終了と同時に再度リクエストを行うと、ステータスコード：429が返されることがあるため
                            for (var i = radioAPI.SONGTIMES.PLAYED; i <= radioAPI.SONGTIMES.DURATION + 5; i++)
                            {
                                // 1秒待機
                                await Task.Delay(1000);

                                // 経過時間と総再生時間を TimeSpan に変換して、mm:ss 形式の文字列にフォーマットしてプロパティに反映
                                Time = $"{TimeSpan.FromSeconds(i + 1 - 4).ToString(@"m\:ss")}/{TimeSpan.FromSeconds(radioAPI.SONGTIMES.DURATION).ToString(@"m\:ss")}";

                                // 現在の楽曲の再生位置を表すスライダーの値を更新
                                TimeSliderValue = i + 1 - 4;
                            }
                        }
                        // アルバム名が「album」である = インターミッション
                        else
                        {
                            TimeSliderMaxValue = 40;

                            // 現在の再生時間が40秒になるまで繰り返し
                            for (var i = radioAPI.SONGTIMES.PLAYED; i <= 40; i++)
                            {
                                // 1秒待機
                                await Task.Delay(1000);

                                // 経過時間と総再生時間を TimeSpan に変換して、mm:ss 形式の文字列にフォーマットしてプロパティに反映
                                Time = $"{TimeSpan.FromSeconds(i + 1 - 4).ToString(@"m\:ss")}/{TimeSpan.FromSeconds(40).ToString(@"m\:ss")}";

                                // 現在の楽曲の再生位置を表すスライダーの値を更新
                                TimeSliderValue = i + 1 - 4;
                            }
                        }
                    }
                }
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 画像を非同期でダウンロードし、UIスレッドで<see cref="BitmapImage"/>オブジェクトを作成して返すメソッドです
        /// </summary>
        /// <param name="url">画像のURL</param>
        /// <returns>作成された<see cref="BitmapImage"/>オブジェクト</returns>
        private static async Task<BitmapImage> DownloadImageAsync(string url)
        {
            // 画像を非同期でダウンロードしてバイト配列として取得
            var bytes = await DownloadImageBytesAsync(url).ConfigureAwait(false);

            // ダウンロードしたバイト配列からBitmapImageを作成して返す
            // UIスレッドで実行する必要があるため、Dispatcherを使用してUIスレッドで処理を行う
            return await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return CreateBitmapImageFromBytes(bytes);
            });
        }

        /// <summary>
        /// 指定された URL から画像を非同期でダウンロードし、バイト配列として返すメソッドです
        /// </summary>
        /// <param name="url">画像のURL</param>
        /// <returns>画像データを含むバイト配列</returns>
        private static async Task<byte[]> DownloadImageBytesAsync(string url)
        {
            try
            {
                // 指定された URL から画像を非同期でダウンロードし、バイト配列として返す
                return await httpClient.GetByteArrayAsync(url);
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定されたバイト配列から新しい <see cref="BitmapImage"/> インスタンスを作成します
        /// </summary>
        /// <remarks><see cref="BitmapImage"/> の読み込み時に例外が発生した場合、エラーメッセージが表示され、<see langword="null"/> が返されますfreezing が <see
        /// langword="true"/> かつ <see cref="BitmapImage"/> がフリーズ可能な場合、返されるインスタンスはフリーズされます</remarks>
        /// <param name="imageBytes">画像データを含むバイト配列<see langword="null"/> または無効なデータの場合は <see langword="null"/> を返します</param>
        /// <param name="freezing">作成した <see cref="BitmapImage"/> をフリーズするかどうかを指定します既定値は <see langword="true"/> ですフリーズすると、スレッドセーフになりパフォーマンスが向上します</param>
        /// <returns>作成された <see cref="BitmapImage"/>imageBytes が無効な場合やエラーが発生した場合は <see langword="null"/></returns>
        private static BitmapImage CreateBitmapImageFromBytes(byte[] imageBytes, bool freezing = true)
        {
            try
            {
                // バイト配列から MemoryStream を作成
                using var memoryStream = new MemoryStream(imageBytes);

                // BitmapImage を作成
                var bitmapImage = new BitmapImage();

                // BitmapImage の初期化を開始
                bitmapImage.BeginInit();

                // キャッシュオプションを OnLoad に設定して、ストリームを閉じた後も BitmapImage が画像データにアクセスできるようにする
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                // ストリームを BitmapImage のソースとして設定
                bitmapImage.StreamSource = memoryStream;

                // BitmapImage の初期化を終了して、画像データの読み込みを完了
                bitmapImage.EndInit();

                // 画像の読み込みが完了した後、必要に応じて BitmapImage をフリーズしてスレッドセーフにする
                if (freezing && bitmapImage.CanFreeze)
                {
                    // BitmapImage をフリーズして、スレッドセーフにし、パフォーマンスを向上させる
                    bitmapImage.Freeze();
                }

                // 作成された BitmapImage を返す
                return bitmapImage;
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定された回数だけリトライを行いながら楽曲情報を取得する非同期メソッドです
        /// </summary>
        /// <param name="maxRetries">最大リトライ回数</param>
        /// <returns>HTTPレスポンスメッセージ</returns>
        /// <exception cref="HttpRequestException">HTTPリクエストに失敗した場合</exception>
        private static async Task<HttpResponseMessage> GetSongInfoWithRetryAsync(int maxRetries = 3)
        {
            var retryCount = 0;

            // リトライ間隔を初期化（例: 1秒）
            var delayMilliseconds = 1000;

            while (retryCount < maxRetries)
            {
                var response = await httpClient.GetAsync(Constants.PLAYING_API_URL);

                // HTTPレスポンスの内容をコンソールに出力
                Console.WriteLine($"レスポンスの内容{Environment.NewLine}{await response.Content.ReadAsStringAsync()}");

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    await Task.Delay(delayMilliseconds);

                    // 次回のリトライは倍の待機時間
                    delayMilliseconds *= 2;
                    retryCount++;
                }
            }
            throw new HttpRequestException("最大リトライ回数に達しました");
        }

        /// <summary>
        /// 再生状態切り替え
        /// </summary>
        private void ChangePlaybackState()
        {
            try
            {
                // 再生中
                if (waveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                    // 一時停止
                    waveOutEvent.Pause();
                    IsPlaying = false;
                }
                // 再生されていない
                else
                {
                    // 再生
                    waveOutEvent.Play();
                    IsPlaying = true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// アルバムアートがクリックされたときの処理を行うメソッドです
        /// </summary>
        private void ClickedAlbumArt()
        {
            try
            {
                // アルバムIDが空でない
                if (!string.IsNullOrWhiteSpace(radioAPI.SONGDATA.ALBUMID.ToString()))
                {
                    // アルバム情報のページの URL を構築
                    var albumInfoUrl = $"{Constants.ALBUM_INFO_PREFIX}{radioAPI.SONGDATA.ALBUMID}/";

                    // アルバム情報のページを開くかどうかをユーザーに確認するメッセージボックスを表示し、ユーザーが「はい」を選択した
                    if (MessageBoxHelper.ShowQuestionMessageBox("アルバム情報のページを開きますか？") == MessageBoxResult.Yes)
                    {
                        // アルバム情報のページを既定のブラウザで開く
                        using var _ = Process.Start(new ProcessStartInfo()
                        {
                            // URL をファイル名として指定
                            FileName = albumInfoUrl,

                            // URL を既定のブラウザで開く
                            UseShellExecute = true
                        });
                    }
                    // ユーザーが「いいえ」を選択した
                    else
                    {
                        return;
                    }
                }
                // アルバムIDが空
                else
                {
                    // アルバム情報のページを開くことができないことをエラーメッセージボックスに表示
                    MessageBoxHelper.ShowErrorMessageBox($"幻想郷ラジオ側にアルバム情報が登録されていません。");
                }
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// シャットダウン処理
        /// </summary>
        private void Shutdown()
        {
            try
            {
                if (startRadioTask != null && startRadioCancellationTokenSource != null)
                {
                    startRadioCancellationTokenSource.Cancel();
                    startRadioTask.Wait();
                    startRadioCancellationTokenSource.Dispose();
                }
                SaveSetting();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 設定を保存するメソッドです
        /// </summary>
        private void SaveSetting()
        {
            try
            {
                // 設定値をフィールドの settingInfo に反映
                _settingInfo.Volume = (int)Volume;

                // 設定ファイルに設定値を書き込む
                JsonFileManager.SaveSetting(Constants.SETTING_FILE_NAME, _settingInfo);

                // アプリケーションを正常終了
                Application.Current.Shutdown(0);
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                MessageBoxHelper.ShowExceptionMessageBox(exception);

                // アプリケーションを異常終了
                Application.Current.Shutdown(1);
            }
        }
    }
}