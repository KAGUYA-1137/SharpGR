using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using SharpGR.Commands;
using SharpGR.FileIO;
using SharpGR.Property;
using static SharpGR.Helper.MessageBoxHelper;

namespace SharpGR.ViewModels
{
    /// <summary>
    /// <see cref="MainWindow"/>のViewModel を表します。
    /// UI のバインディング用プロパティ（例: <see cref="AppTitle"/>）を公開し、
    /// <see cref="MainWindow"/>と<see cref="MainViewModel"/>の橋渡しを行います。
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        #region プロパティ

        /// <summary>
        /// タイトルバーに表示する文字列を取得または設定します。
        /// </summary>
        public string AppTitle
        {
            get => appTitle;
            set => SetProperty(ref appTitle, value);
        }
        private string appTitle = string.Empty;

        /// <summary>
        /// 音量を取得または設定します。
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
                    _waveOutEvent.Volume = (float)(volume / 100.0);
                }
            }
        }
        private double volume = 0.0;

        /// <summary>
        /// アルバムアートを取得または設定します。
        /// </summary>
        public BitmapImage AlbumArt
        {
            get => albumArt;
            set => SetProperty(ref albumArt, value);
        }
        private BitmapImage albumArt = null;

        /// <summary>
        /// 楽曲名を取得または設定します。
        /// </summary>
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }
        private string title = Constants.Blank;

        /// <summary>
        /// アーティスト名を取得または設定します。
        /// </summary>
        public string Artist
        {
            get => artist;
            set => SetProperty(ref artist, value);
        }
        private string artist = Constants.Blank;

        /// <summary>
        /// アルバムIDを取得または設定します。
        /// </summary>
        public string Album
        {
            get => album;
            set => SetProperty(ref album, value);
        }
        private string album = Constants.Blank;

        /// <summary>
        /// 経過時間と総再生時間を取得または設定します。
        /// </summary>
        public string Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }
        private string time = Constants.BlankTimer;

        /// <summary>
        /// 現在の楽曲の再生位置を表すスライダーの値を取得または設定します。
        /// </summary>
        public double TimeSliderValue
        {
            get => timeSliderValue;
            set => SetProperty(ref timeSliderValue, value);
        }
        private double timeSliderValue = 0.0;

        /// <summary>
        /// 現在の楽曲の再生位置を表すスライダーの最大値を取得または設定します。
        /// </summary>
        public double TimeSliderMaxValue
        {
            get => timeSliderMaxValue;
            set => SetProperty(ref timeSliderMaxValue, value);
        }
        private double timeSliderMaxValue = 0.0;

        /// <summary>
        /// 再生中かどうかを取得または設定します。
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
        /// JSONファイルへの書き込みと読み込みを行うためのインスタンスです。
        /// </summary>
        private readonly JsonUtility _jsonUtility = new JsonUtility();

        /// <summary>
        /// アプリケーションの設定値を管理するためのインスタンスです。
        /// </summary>
        private readonly SettingInfo _settingInfo = new SettingInfo();

        /// <summary>
        /// オーディオストリームを読み取るためのインスタンスです。
        /// <see cref="Constants.StreamEndpoint"/>で指定されたストリームエンドポイントからオーディオデータを読み取ります。
        /// </summary>
        private readonly MediaFoundationReader _mediaFoundationReader = new MediaFoundationReader(Constants.StreamEndpoint);

        /// <summary>
        /// NAudio の出力デバイスを管理するためのインスタンスです。
        /// 再生操作に使用します。
        /// </summary>
        /// <remarks>
        /// このインスタンスはコンストラクタで初期化されます。破棄処理（Dispose）は
        /// ライフサイクルに応じて必要に応じて実装してください。
        /// </remarks>
        public WaveOutEvent _waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// HTTP リクエストを送信するための <see cref="HttpClient"/> の共有インスタンスです。
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 幻想郷ラジオのAPIから取得した楽曲情報を格納するためのインスタンスです。
        /// </summary>
        private RadioAPI _radioAPI;

        #endregion

        #region コマンド

        /// <summary>
        /// 再生状態を変更するコマンドを取得します。
        /// </summary>
        public DelegateCommand ChangePlaybackStateCommand { get; private set; }

        /// <summary>
        /// アルバム情報を表示するコマンドを取得します。
        /// </summary>
        public DelegateCommand OpenAlbumInfoCommand { get; private set; }

        /// <summary>
        /// 設定を保存するコマンドを取得します。
        /// </summary>
        public DelegateCommand SaveSettingCommand { get; private set; }

        #endregion

        /// <summary>
        /// 新しい <see cref="MainViewModel"/> のインスタンスを初期化します。
        /// </summary>
        public MainViewModel()
        {
            try
            {
                // コマンドとメソッドの関連付け
                ChangePlaybackStateCommand = new DelegateCommand(ChangePlaybackState);
                OpenAlbumInfoCommand = new DelegateCommand(ClickedAlbumArt);
                SaveSettingCommand = new DelegateCommand(SaveSetting);

                // WaveOutEvent を MediaFoundationReader で初期化
                _waveOutEvent.Init(_mediaFoundationReader);

                // アセンブリの情報を取得
                var assembly = Assembly.GetExecutingAssembly().GetName();

                // アセンブリのバージョン情報を取得
                var version = assembly.Version;

                // アプリケーションのタイトルをアセンブリの名前とバージョンから構築
                AppTitle = $"{assembly.Name} {version.Major}.{version.Minor}";

                // 設定ファイルの保存先ディレクトリが存在しない場合
                if (!Directory.Exists(Constants.SettingFooter))
                {
                    // 設定ファイルの保存先ディレクトリを作成
                    Directory.CreateDirectory(Constants.SettingFooter);
                }

                // 設定ファイルが存在しない場合
                if (!File.Exists(Constants.MainWindowSettingFileName))
                {
                    // デフォルトの設定値を使用して設定ファイルを作成
                    _jsonUtility.WriteToJson(Constants.MainWindowSettingFileName, _settingInfo);
                }

                // 設定ファイルから設定値を読み込む
                var settingInfo = _jsonUtility.ReadSettingFromJson(Constants.MainWindowSettingFileName);

                // 設定値が null でない
                if (settingInfo != null)
                {
                    // 読み込んだ設定値をフィールドの settingInfo に反映
                    _settingInfo = settingInfo;

                    // 設定値をメイン画面に反映
                    Volume = settingInfo.Volume;

                    // 音量を0.0から1.0の範囲に変換して反映
                    _waveOutEvent.Volume = (float)(settingInfo.Volume / 100.0);

                    // 再生の開始を非同期で行う
                    StartRadio();
                }
                // 設定値が null = 設定ファイルの読み込みに失敗した
                else
                {
                    // 設定ファイルの読み込みに失敗したことをエラーメッセージボックスに表示
                    ShowErrorMessageBox("設定ファイルの読み込みに失敗しました。\nデフォルトの設定値を使用します。");

                    // デフォルトの設定値を使用して反映
                    Volume = _settingInfo.Volume;
                    _waveOutEvent.Volume = (float)(_settingInfo.Volume / 100.0);

                    // 再生の開始を非同期で行う
                    StartRadio();
                }
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");

                // 起動時の例外発生は致命的なため、アプリケーションを終了
                Application.Current.Shutdown(1);
            }
        }

        /// <summary>
        /// 再生を開始するメソッドです。
        /// オーディオストリームの再生を開始し、楽曲情報の取得も非同期で行います。
        /// </summary>
        private async void StartRadio()
        {
            try
            {
                // 再生の開始
                _waveOutEvent.Play();

                IsPlaying = true;

                // アセンブリの情報を取得
                var assembly = Assembly.GetExecutingAssembly().GetName();

                // アセンブリのバージョン情報を取得
                var version = assembly.Version;

                // HTTPリクエストの User-Agent ヘッダーにアプリケーションの名前とバージョンを追加
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{assembly.Name} {version.Major}.{version.Minor} (https://github.com/KAGUYA-1137/SharpGR)");

                // アプリケーションが起動している間
                while (true)
                {
                    // 楽曲情報の取得を非同期で行う
                    await GetSongInfoAsync();
                }
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");
            }
        }

        /// <summary>
        /// 楽曲情報を非同期で取得し、プロパティに反映させるメソッドです。
        /// </summary>
        /// <returns><see cref="Task"/> オブジェクト</returns>
        private async Task GetSongInfoAsync()
        {
            try
            {
                // HTTP GETリクエストを作成
                using var request = new HttpRequestMessage(HttpMethod.Get, Constants.RadioAPIURL);

                // 既にUser-Agentヘッダーが存在する
                if (request.Headers.UserAgent.Count > 0)
                {
                    // User-Agentヘッダーをクリア
                    request.Headers.UserAgent.Clear();
                }

                // HTTPリクエストのUser-Agentヘッダーにアプリケーションの名前とバージョンを追加
                request.Headers.UserAgent.ParseAdd(_httpClient.DefaultRequestHeaders.UserAgent.ToString());

                // HTTPリクエストの内容をコンソールに出力
                Console.WriteLine($"リクエストの内容\n{request}");

                // HTTPリクエストを楽曲情報の取得先に対して送信し、レスポンスを受け取る
                using (var response = await GetSongInfoWithRetryAsync())
                {
                    // 取得した JSON データを RadioAPI クラスのインスタンスに変換
                    _radioAPI = _jsonUtility.ParseFromResponse(await response.Content.ReadAsStringAsync());
                }

                // 変換した RadioAPI インスタンスが null ではない
                if (_radioAPI != null)
                {
                    // アルバムアートの URL を取得し、プロパティに反映
                    var albumArtUrl = _radioAPI.MISC.ALBUMART;

                    // アルバムアートの URL が空でない
                    if (!string.IsNullOrWhiteSpace(albumArtUrl))
                    {
                        // アルバムアートの URL を使用して画像を取得し、プロパティに反映
                        AlbumArt = await DownloadImageAsync($"{Constants.AlbumArtPrefixURL}{albumArtUrl}");
                    }
                    // アルバムアートの URL が空
                    else
                    {
                        // プレースホルダー画像をプロパティに反映
                        AlbumArt = await DownloadImageAsync(Constants.PlaceholderAlbumArtURL);
                    }

                    // 楽曲情報をプロパティに反映
                    Title = _radioAPI.SONGINFO.TITLE;
                    Artist = _radioAPI.SONGINFO.ARTIST;
                    Album = _radioAPI.SONGINFO.ALBUM;

                    // アルバム名が「album」でない = インターミッションではない
                    if (_radioAPI.SONGINFO.ALBUM != "album")
                    {
                        TimeSliderMaxValue = _radioAPI.SONGTIMES.DURATION;

                        // 現在の再生時間が楽曲の総再生時間に5秒足した秒数になるまで繰り返し
                        // 5秒足すのは楽曲の終了と同時に再度リクエストを行うと、ステータスコード：429が返されることがあるため
                        for (var i = _radioAPI.SONGTIMES.PLAYED; i <= _radioAPI.SONGTIMES.DURATION + 5; i++)
                        {
                            // 1秒待機
                            await Task.Delay(1000);

                            // 経過時間と総再生時間を TimeSpan に変換して、mm:ss 形式の文字列にフォーマットしてプロパティに反映
                            Time = $"{TimeSpan.FromSeconds(i + 1 - 4).ToString(@"m\:ss")}/{TimeSpan.FromSeconds(_radioAPI.SONGTIMES.DURATION).ToString(@"m\:ss")}";

                            // 現在の楽曲の再生位置を表すスライダーの値を更新
                            TimeSliderValue = i + 1 - 4;
                        }
                    }
                    // アルバム名が「album」である = インターミッション
                    else
                    {
                        TimeSliderMaxValue = 40;

                        // 現在の再生時間が40秒になるまで繰り返し
                        for (var i = _radioAPI.SONGTIMES.PLAYED; i <= 40; i++)
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
                // 変換した RadioAPI インスタンスが null
                else
                {
                    // 取得した楽曲情報が空であることを示す例外をスロー
                    throw new InvalidOperationException("幻想郷ラジオから取得した楽曲情報が空です。");
                }
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");
            }
        }

        /// <summary>
        /// 画像を非同期でダウンロードし、UIスレッドで<see cref="BitmapImage"/>オブジェクトを作成して返すメソッドです。
        /// </summary>
        /// <param name="url">画像のURL</param>
        /// <returns>作成された<see cref="BitmapImage"/>オブジェクト</returns>
        private async Task<BitmapImage> DownloadImageAsync(string url)
        {
            // 画像を非同期でダウンロードしてバイト配列として取得
            var bytes = await DownloadImageBytesAsync(url).ConfigureAwait(false);

            // ダウンロードしたバイト配列からBitmapImageを作成して返す。
            // UIスレッドで実行する必要があるため、Dispatcherを使用してUIスレッドで処理を行う。
            return await Application.Current.Dispatcher.InvokeAsync(() => CreateBitmapImageFromBytes(bytes));
        }

        /// <summary>
        /// 指定された URL から画像を非同期でダウンロードし、バイト配列として返すメソッドです。
        /// </summary>
        /// <param name="url">画像のURL</param>
        /// <returns>画像データを含むバイト配列</returns>
        private async Task<byte[]> DownloadImageBytesAsync(string url)
        {
            try
            {
                // 指定された URL から画像を非同期でダウンロードし、バイト配列として返す
                return await _httpClient.GetByteArrayAsync(url);
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 指定されたバイト配列から新しい <see cref="BitmapImage"/> インスタンスを作成します。
        /// </summary>
        /// <remarks><see cref="BitmapImage"/> の読み込み時に例外が発生した場合、エラーメッセージが表示され、<see langword="null"/> が返されます。freezing が <see
        /// langword="true"/> かつ <see cref="BitmapImage"/> がフリーズ可能な場合、返されるインスタンスはフリーズされます。</remarks>
        /// <param name="imageBytes">画像データを含むバイト配列。<see langword="null"/> または無効なデータの場合は <see langword="null"/> を返します。</param>
        /// <param name="freezing">作成した <see cref="BitmapImage"/> をフリーズするかどうかを指定します。既定値は <see langword="true"/> です。フリーズすると、スレッドセーフになりパフォーマンスが向上します。</param>
        /// <returns>作成された <see cref="BitmapImage"/>。imageBytes が無効な場合やエラーが発生した場合は <see langword="null"/>。</returns>
        private static BitmapImage CreateBitmapImageFromBytes(byte[] imageBytes, bool freezing = true)
        {
            try
            {
                // バイト配列が <see langword="null"/> または空の場合は <see langword="null"/> を返す
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
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 指定された回数だけリトライを行いながら楽曲情報を取得する非同期メソッドです。
        /// </summary>
        /// <param name="maxRetries">最大リトライ回数</param>
        /// <returns>HTTPレスポンスメッセージ</returns>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<HttpResponseMessage> GetSongInfoWithRetryAsync(int maxRetries = 3)
        {
            var retryCount = 0;

            // リトライ間隔を初期化（例: 1秒）
            var delayMilliseconds = 1000;

            while (retryCount < maxRetries)
            {
                var response = await _httpClient.GetAsync(Constants.RadioAPIURL);

                // HTTPレスポンスの内容をコンソールに出力
                Console.WriteLine($"レスポンスの内容\n{await response.Content.ReadAsStringAsync()}");

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    await Task.Delay(delayMilliseconds);
                    delayMilliseconds *= 2; // 次回のリトライは倍の待機時間
                    retryCount++;
                }
            }
            throw new HttpRequestException("最大リトライ回数に達しました。");
        }

        /// <summary>
        /// 再生状態切り替え
        /// </summary>
        private void ChangePlaybackState()
        {
            try
            {
                // 再生中
                if (_waveOutEvent.PlaybackState == PlaybackState.Playing)
                {
                    // 一時停止
                    _waveOutEvent.Pause();
                    IsPlaying = false;
                }
                // 再生されていない
                else
                {
                    // 再生
                    _waveOutEvent.Play();
                    IsPlaying = true;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox($"再生状態の切り替えに失敗:{ex.StackTrace}\n\n{ex.Message}");
            }
        }

        /// <summary>
        /// アルバムアートがクリックされたときの処理を行うメソッドです。
        /// </summary>
        private void ClickedAlbumArt()
        {
            try
            {
                // アルバムIDが空でない
                if (!string.IsNullOrWhiteSpace(_radioAPI.SONGDATA.ALBUMID.ToString()))
                {
                    // アルバム情報のページの URL を構築
                    var albumInfoUrl = $"{Constants.AlbumInfoURL}{_radioAPI.SONGDATA.ALBUMID}/";

                    // アルバム情報のページを開くかどうかをユーザーに確認するメッセージボックスを表示し、ユーザーが「はい」を選択した
                    if (ShowQuestionMessageBox("アルバム情報のページを開きますか？") == MessageBoxResult.Yes)
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
                    ShowErrorMessageBox("アルバム情報のページを開くことができません。\nアルバムIDが空です。");
                }
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");
            }
        }

        /// <summary>
        /// 設定を保存するメソッドです。
        /// </summary>
        private void SaveSetting()
        {
            try
            {
                // 設定値をフィールドの settingInfo に反映
                _settingInfo.Volume = (int)Volume;

                // 設定ファイルに設定値を書き込む
                _jsonUtility.WriteToJson(Constants.MainWindowSettingFileName, _settingInfo);

                // アプリケーションを正常終了
                Application.Current.Shutdown(0);
            }
            // 例外が発生した
            catch (Exception exception)
            {
                // 例外の内容をエラーメッセージボックスに表示
                ShowErrorMessageBox($"{exception.Message}\n\n{exception.StackTrace}");

                // アプリケーションを異常終了
                Application.Current.Shutdown(1);
            }
        }
    }
}