using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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
                    waveOutEvent.Volume = (float)(volume / 100.0);
                }
            }
        }
        private double volume = 0.0;

        /// <summary>
        /// アルバムアートを取得または設定します。
        /// </summary>
        public ImageSource AlbumArt
        {
            get => albumArt;
            set => SetProperty(ref albumArt, value);
        }
        private ImageSource albumArt = null;

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

        #endregion

        #region フィールド

        /// <summary>
        /// JSONファイルへの書き込みと読み込みを行うためのインスタンスです。
        /// </summary>
        private readonly JsonUtility jsonUtility = new JsonUtility();

        /// <summary>
        /// アプリケーションの設定値を管理するためのインスタンスです。
        /// </summary>
        private readonly SettingInfo settingInfo = new SettingInfo();

        /// <summary>
        /// オーディオストリームを読み取るためのインスタンスです。
        /// <see cref="Constants.StreamEndpoint"/>で指定されたストリームエンドポイントからオーディオデータを読み取ります。
        /// </summary>
        private readonly MediaFoundationReader mediaFoundationReader = new MediaFoundationReader(Constants.StreamEndpoint);

        /// <summary>
        /// NAudio の出力デバイスを管理するためのインスタンスです。
        /// 再生操作に使用します。
        /// </summary>
        /// <remarks>
        /// このインスタンスはコンストラクタで初期化されます。破棄処理（Dispose）は
        /// ライフサイクルに応じて必要に応じて実装してください。
        /// </remarks>
        public WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// HTTP リクエストを送信するための <see cref="HttpClient"/> の共有インスタンスです。
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 幻想郷ラジオのAPIから取得した楽曲情報を格納するためのインスタンスです。
        /// </summary>
        private RadioAPI radioAPI;

        #endregion

        #region コマンド

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
                OpenAlbumInfoCommand = new DelegateCommand(ClickedAlbumArt);
                SaveSettingCommand = new DelegateCommand(SaveSetting);

                // WaveOutEvent を MediaFoundationReader で初期化
                waveOutEvent.Init(mediaFoundationReader);

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
                    jsonUtility.WriteToJson(Constants.MainWindowSettingFileName, this.settingInfo);
                }

                // 設定ファイルから設定値を読み込む
                var settingInfo = jsonUtility.ReadSettingFromJson(Constants.MainWindowSettingFileName);

                // 設定値が null でない
                if (settingInfo != null)
                {
                    // 読み込んだ設定値をフィールドの settingInfo に反映
                    this.settingInfo = settingInfo;

                    // 設定値をメイン画面に反映
                    Volume = settingInfo.Volume;

                    // 音量を0.0から1.0の範囲に変換して反映
                    waveOutEvent.Volume = (float)(settingInfo.Volume / 100.0);

                    // 再生の開始を非同期で行う
                    StartRadio();
                }
                // 設定値が null = 設定ファイルの読み込みに失敗した
                else
                {
                    // 設定ファイルの読み込みに失敗したことをエラーメッセージボックスに表示
                    ShowErrorMessageBox("設定ファイルの読み込みに失敗しました。\nデフォルトの設定値を使用します。");

                    // デフォルトの設定値を使用して反映
                    Volume = this.settingInfo.Volume;
                    waveOutEvent.Volume = (float)(this.settingInfo.Volume / 100.0);

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
                waveOutEvent.Play();

                // アセンブリの情報を取得
                var assembly = Assembly.GetExecutingAssembly().GetName();

                // アセンブリのバージョン情報を取得
                var version = assembly.Version;

                // HTTPリクエストの User-Agent ヘッダーにアプリケーションの名前とバージョンを追加
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{assembly.Name} {version.Major}.{version.Minor} (https://github.com/KAGUYA-1137/SharpGR)");

                // アプリケーションが起動している間
                while (true)
                {
                    // 楽曲情報の取得を非同期で行う
                    await GetSonInfoAsync();
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
        private async Task GetSonInfoAsync()
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
                request.Headers.UserAgent.ParseAdd(httpClient.DefaultRequestHeaders.UserAgent.ToString());

                // HTTPリクエストの内容をコンソールに出力
                Console.WriteLine($"リクエストの内容\n{request}");

                // HTTPリクエストを楽曲情報の取得先に対して送信し、レスポンスを受け取る
                using (var response = await GetSongInfoWithRetryAsync())
                {
                    // 取得した JSON データを RadioAPI クラスのインスタンスに変換
                    radioAPI = jsonUtility.ParseFromResponse(await response.Content.ReadAsStringAsync());
                }

                // 変換した RadioAPI インスタンスが null ではない
                if (radioAPI != null)
                {
                    // アルバムアートの URL を取得し、プロパティに反映
                    var albumArtUrl = radioAPI.MISC.ALBUMART;

                    // アルバムアートの URL が空でない
                    if (!string.IsNullOrWhiteSpace(albumArtUrl))
                    {
                        // アルバムアートの URL を使用して画像を取得し、プロパティに反映
                        AlbumArt = (ImageSource)new ImageSourceConverter().ConvertFromString($"{Constants.AlbumArtPrefixURL}{albumArtUrl}");
                    }
                    // アルバムアートの URL が空
                    else
                    {
                        // プレースホルダー画像をプロパティに反映
                        AlbumArt = (ImageSource)new ImageSourceConverter().ConvertFromString(Constants.PlaceholderAlbumArtURL);
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
                            Time = $"{TimeSpan.FromSeconds(i + 1 - 5).ToString(@"m\:ss")}/{TimeSpan.FromSeconds(radioAPI.SONGTIMES.DURATION).ToString(@"m\:ss")}";

                            // 現在の楽曲の再生位置を表すスライダーの値を更新
                            TimeSliderValue = i + 1 - 5;
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
                            Time = $"{TimeSpan.FromSeconds(i + 1 - 5).ToString(@"m\:ss")}/{TimeSpan.FromSeconds(40).ToString(@"m\:ss")}";

                            // 現在の楽曲の再生位置を表すスライダーの値を更新
                            TimeSliderValue = i + 1 - 5;
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
                var response = await httpClient.GetAsync(Constants.RadioAPIURL);

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
        /// アルバムアートがクリックされたときの処理を行うメソッドです。
        /// </summary>
        private void ClickedAlbumArt()
        {
            try
            {
                // アルバムIDが空でない
                if (!string.IsNullOrWhiteSpace(radioAPI.SONGDATA.ALBUMID.ToString()))
                {
                    // アルバム情報のページの URL を構築
                    var albumInfoUrl = $"{Constants.AlbumInfoURL}{radioAPI.SONGDATA.ALBUMID}/";

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
                settingInfo.Volume = (int)Volume;

                // 設定ファイルに設定値を書き込む
                jsonUtility.WriteToJson(Constants.MainWindowSettingFileName, settingInfo);

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