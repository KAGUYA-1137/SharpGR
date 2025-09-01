using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.Wave;
using NLog;
using SharpGR_WPF.Commands;
using SharpGR_WPF.IO;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SharpGR_WPF.ViewModels
{
    /// <summary>
    /// MainWindowのビューモデル
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region Variable

        public WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// 任意のファイルを読み込むクラス
        /// </summary>
        private MediaFoundationReader mediaFoundationReader;

        /// <summary>
        /// 設定ファイルの構造
        /// </summary>
        private SettingInfo settingInfo = new SettingInfo();

        /// <summary>
        /// HTTP/HTTPS通信を行うクラス
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 再生中の楽曲の総再生時間
        /// </summary>
        public int Duration;

        /// <summary>
        /// 現在の再生時間
        /// </summary>
        public int Played;

        /// <summary>
        /// 幻想郷ラジオのAPI
        /// </summary>
        public RadioAPI RadioAPI = new RadioAPI();

        /// <summary>
        /// ログ出力を行うクラス
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Timer timer = new Timer();

        /// <summary>
        /// タイトルバーに表示する初期の文字列
        /// </summary>
        private string appTitle = string.Empty;

        /// <summary>
        /// タイトルバーに表示する新しい文字列
        /// </summary>
        public string AppTitle
        {
            get => appTitle;
            set
            {
                appTitle = value;
                SetProperty();
            }
        }

        /// <summary>
        /// 前回の音量値の保存用
        /// </summary>
        private double previousVolume;

        /// <summary>
        /// 音量テキストボックスに表示する文字列
        /// </summary>
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                if (double.TryParse(value.ToString(), out double newVolume) && IsValidRange(newVolume))
                {
                    previousVolume = newVolume;
                    SetProperty();
                    UpdateNAudioVolume();
                }
                else
                {
                    volume = previousVolume;
                    SetProperty();
                    //MessageBox.Show("無効な値です");
                }
            }
        }
        private double volume = 100;

        private string playButtonText = "再生";

        public string PlayButtonText
        {
            get => playButtonText;
            set
            {
                playButtonText = value;
                SetProperty();
            }
        }


        //private int timeSliderMax = 100;

        public int TimeSliderMax
        {
            get => Duration;
            set
            {
                Duration = value;
                SetProperty();
            }
        }

        //private int timeSliderValue = 0;

        public int TimeSliderValue
        {
            get => Played;
            set
            {
                Played = value;
                SetProperty();
            }
        }

        private string timeLabelText = Constants.BlankTimer;

        public string TimeLabelText
        {
            get => timeLabelText;
            set
            {
                timeLabelText = value;
                SetProperty();
            }
        }

        private string nameLabelText = Constants.Blank;

        public string NameLabelText
        {
            get => nameLabelText;
            set
            {
                nameLabelText = value;
                SetProperty();
            }
        }

        private string artistLabelText = Constants.Blank;

        public string ArtistLabelText
        {
            get => artistLabelText;
            set
            {
                artistLabelText = value;
                SetProperty();
            }
        }

        private string albumNameLabelText = Constants.Blank;

        public string AlbumNameLabelText
        {
            get => albumNameLabelText;
            set
            {
                albumNameLabelText = value;
                SetProperty();
            }
        }

        private ImageSource albumArtImageSource = null;

        public ImageSource AlbumArtImageSource
        {
            get => albumArtImageSource;
            set
            {
                albumArtImageSource = value;
                SetProperty();
            }
        }

        private readonly DispatcherTimer dispatcherTimer;

        private JsonUtility jsonUtility;

        /// <summary>
        /// 楽曲情報の取得が初めてかどうか
        /// ログメッセージを分けるために使用
        /// </summary>
        private bool isFirstGet = true;

        #endregion

        #region DelegateCommand

        /// <summary>
        /// 再生ボタンクリック時のイベント
        /// </summary>
        public DelegateCommand PlayButtonClickCommand { get; private set; }

        /// <summary>
        /// アルバムアートクリック時のイベント
        /// </summary>
        public DelegateCommand ArtWorkClickCommand { get; private set; }

        /// <summary>
        /// 画面を閉じるコマンド
        /// </summary>
        public DelegateCommand CloseCommand { get; private set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            // 再生ボタンクリック時のイベントにメソッドをバインド
            PlayButtonClickCommand = new DelegateCommand(PlayButtonClick);

            // アルバムアートクリック時のイベントにメソッドをバインド
            ArtWorkClickCommand = new DelegateCommand(ArtWorkClick);

            CloseCommand = new DelegateCommand(CloseWindow);

            // タイトルバーに表示する文字列を設定
            AppTitle = $"{Assembly.GetExecutingAssembly().GetName().Name} Ver.{Assembly.GetExecutingAssembly().GetName().Version}";

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);

            ReadSettingsAsync();
        }

        /// <summary>
        /// メイン画面の設定ファイルの読み込み
        /// </summary>
        private void ReadSettingsAsync()
        {
            try
            {
                // 設定ファイルのディレクトリが存在しない場合
                if (!Directory.Exists(Constants.MainWindowSettingFileName))
                {
                    logger.Warn("設定ファイルのディレクトリが存在しません。");

                    // 設定ファイルのディレクトリを作成
                    var directoryInfo = Directory.CreateDirectory(Constants.SettingFooter);
                    logger.Info($"設定ファイルのディレクトリを「{directoryInfo.FullName}」へ作成しました。");
                }

                jsonUtility = new JsonUtility(this);
                // 設定ファイルが存在しない場合
                if (!File.Exists(Constants.MainWindowSettingFileName))
                {
                    logger.Warn("設定ファイルが存在しません。");

                    // 設定ファイルを作成してデフォルトの設定値を書き込み
                    if (!jsonUtility.WriteJson(Constants.MainWindowSettingFileName, settingInfo))
                    {
                        var errorMessage = "設定ファイルの作成、またはデフォルトの設定値の書き込みに失敗しました。";
                        logger.Warn(errorMessage);
                        MessageBox.Show(errorMessage, "設定ファイルへ書き込めませんでした", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        logger.Info("設定ファイルにデフォルトの設定値を書き込みました。");
                    }
                }

                logger.Info("設定ファイルを読み込みます。");
                jsonUtility.ReadSettingFromJson(Constants.MainWindowSettingFileName);
            }
            catch (Exception ex)
            {
                var errorMessage = "設定の読み込み中にエラーが発生しました";
                MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void PlayButtonClick()
        {
            if (waveOutEvent.PlaybackState != PlaybackState.Playing)
            {
                StartRadio();
            }
            else
            {
                StopRadio();
            }
        }

        /// <summary>
        /// 幻想郷ラジオを再生
        /// </summary>
        public async void StartRadio()
        {
            try
            {
                mediaFoundationReader = new MediaFoundationReader(Constants.StreamEndpoint);
                waveOutEvent.Init(mediaFoundationReader);
                waveOutEvent.Play();
                await GetSongInfoFromAPIAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = "再生中にエラーが発生しました";
                MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ユーザーが入力した音量の範囲チェック
        /// </summary>
        /// <param name="value">ユーザーが入力した音量</param>
        /// <returns>0～100の範囲内なら true、範囲外なら false</returns>
        private bool IsValidRange(double value)
        {
            return value is >= 0 and <= 100;
        }

        /// <summary>
        /// 音量を変更（音量スライダーまたはテキストボックスの内容が変わったときに発動）
        /// </summary>
        private void UpdateNAudioVolume()
        {
            waveOutEvent.Volume = (float)(Volume / 100f);
        }

        ///// <summary>
        ///// 1秒ごとに曲情報取得
        ///// </summary>
        //private async void Timer_Tick(object sender, EventArgs e)
        //{
        //    await GetSongInfoFromAPIAsync();

        //    TimeSliderMax = Duration;

        //    if (Played <= Duration)
        //    {
        //        TimeSliderValue = Played;
        //    }

        //    else
        //    {
        //        Played = 0;
        //        TimeSliderValue = Played;
        //        TimeLabelText = "--:-- / --:--";
        //    }
        //}

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報を非同期で取得して画面へ反映。
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        private async Task GetSongInfoFromAPIAsync()
        {
            // while(true)することで、無限ループさせる。
            // await Task.Delay(TimeSpan.FromSeconds(RadioAPI.SONGTIMES.REMAINING));まで実行されると、REMAININGの時間の間は実行を一時停止する。
            // 時間経過後、楽曲情報を再取得する
            while (true)
            {
                try
                {
                    if (isFirstGet)
                    {
                        logger.Info("幻想郷ラジオで再生中の楽曲情報を取得します。");
                        isFirstGet = false;
                    }
                    else
                    {
                        logger.Info("再生中の楽曲が変更されました。楽曲情報を再取得します。");
                    }

                    // HTTPリクエストメッセージを作成
                    logger.Info("HTTPリクエストメッセージを作成します。");
                    using var request = new HttpRequestMessage(HttpMethod.Get, Constants.RadioAPIURL);
                    //SetStatusMessage("楽曲情報を取得しています...");
                    logger.Info($"HTTPリクエストメッセージを作成しました。\n{request}");

                    // HTTPリクエストを送信
                    logger.Info($"HTTPリクエストメッセージを {request.RequestUri} へ送信します。");
                    using var response = await httpClient.SendAsync(request);
                    logger.Info($"送信したHTTPリクエストメッセージに対して以下のレスポンスメッセージが返されました。\n{response}");

                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info($"HTTPステータスコードは {response.StatusCode} です。");

                        logger.Info("レスポンスメッセージからレスポンスボディを読み込みます。");

                        // レスポンスボディを読み込む
                        var resBody = await response.Content.ReadAsStringAsync();
                        logger.Info($"以下のレスポンスボディを読み込みました。\n{resBody}");

                        jsonUtility.ParseAndSetDataFromResponse(resBody, this);

                        if (RadioAPI.SONGINFO.ALBUM != "album")
                        {
                            logger.Info($"楽曲情報の取得と画面への反映に成功しました。\n次回の楽曲情報の取得と画面への反映は {RadioAPI.SONGTIMES.REMAINING} 秒後に行います。");
                            // 現在の楽曲の再生が終わるまで楽曲情報の再取得を中断（BAN対策）
                            await Task.Delay(TimeSpan.FromSeconds(RadioAPI.SONGTIMES.REMAINING));
                        }
                        else
                        {
                            logger.Info($"楽曲情報の取得と画面への反映に成功しました。\n現在再生中の楽曲はインターミッションであるため、次回の楽曲情報の取得と画面への反映は 30 秒後に行います。");
                            // インターミッションの場合は、いつ終わるかレスポンスボディから読み取れないため、30秒後に再取得
                            await Task.Delay(TimeSpan.FromSeconds(30));
                        }

                        //var newTitle = JsonUtility.GetTitleFromResponse(resBody);

                        //if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != NameLabelText)
                        //{
                        //    // タイトルが変わった時のみパース
                        //    NameLabelText = newTitle;
                        //    jsonUtility.ParseAndSetDataFromResponse(resBody, this);
                        //}

                        //// レスポンスボディをデシリアライズ
                        //radioAPI = JsonUtility.(resBody);

                        //if (radioAPI != null)
                        //{
                        //    logger.Info("メイン画面の各コンポーネントへ楽曲情報を反映します。");
                        //    //SetStatusMessage("楽曲情報を取得しました");

                        //    // デシリアライズしたオブジェクトから楽曲情報を取得
                        //    if (string.IsNullOrWhiteSpace(radioAPI.SONGINFO.TITLE))
                        //    {
                        //        NameLabelText = Constants.Blank;
                        //    }
                        //    NameLabelText = radioAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                        //    logger.Info($"楽曲名「{radioAPI.SONGINFO.TITLE}」を反映しました。");

                        //    if (string.IsNullOrWhiteSpace(radioAPI.SONGINFO.ARTIST))
                        //    {
                        //        ArtistLabelText = Constants.Blank;
                        //    }
                        //    ArtistLabelText = radioAPI.SONGINFO.ARTIST; // アーティスト名を設定
                        //    logger.Info($"アーティスト名「{radioAPI.SONGINFO.ARTIST}」を反映しました。");

                        //    AlbumNameLabelText = radioAPI.SONGINFO.ALBUM;   // アルバム名を設定
                        //    logger.Info($"アルバム名「{radioAPI.SONGINFO.ALBUM}」を反映しました。");

                        //    Duration = radioAPI.SONGTIMES.DURATION;  // 楽曲の総再生時間を設定
                        //    logger.Info($"楽曲の長さ「{TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}」を反映しました。");

                        //    Played = radioAPI.SONGTIMES.PLAYED;  // 現在の再生時間を設定
                        //    logger.Info($"現在の再生時間「{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")}」を反映しました。");

                        //    // アルバムアートが未定義の場合
                        //    if (string.IsNullOrWhiteSpace(radioAPI.MISC.ALBUMART))
                        //    {
                        //        // デフォルトのアルバムアートを表示
                        //        AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(Constants.PlaceholderAlbumArtURL);
                        //    }

                        //    // アルバムアートが定義されている場合
                        //    else
                        //    {
                        //        /* アルバムアート取得先の絶対パスと再生中の楽曲のアルバムアートのファイル名を合体して、
                        //        再生中の楽曲のアルバムアートのパスを特定 */
                        //        AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString($"{Constants.AlbumArtPrefixURL}{radioAPI.MISC.ALBUMART}");
                        //    }
                        //}

                        //// 楽曲情報が空の場合
                        //else
                        //{
                        //    throw new NullReferenceException("楽曲情報を取得できませんでした。");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    var errorMessage = ex.Message;
                    MessageBox.Show(errorMessage, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        /// <summary>
        /// 現在の再生時間を更新（タイマー表示）
        /// </summary>
        private void UpdateCurrentTime(object sender, EventArgs e)
        {
            // 総再生時間と現在の再生時間を表示
            TimeLabelText = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";

            if (Played < Duration)
            {
                Played++;
            }
            else
            {
                Played = 0;
            }
        }

        private void ArtWorkClick()
        {
            if (!string.IsNullOrWhiteSpace(RadioAPI.SONGDATA.ALBUMID.ToString()))
            {
                var albumInfoPath = $"{Constants.AlbumInfoURL}{RadioAPI.SONGDATA.ALBUMID}/";

                if (MessageBox.Show(albumInfoPath, "アルバム情報をブラウザーで確認しますか？", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    using var process = new Process();
                    process.StartInfo.FileName = albumInfoPath;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
                else
                {
                    return;
                }
            }

            else
            {
                _ = MessageBox.Show("このアルバムの情報はありません", "アルバム情報なし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CloseWindow()
        {
            if (PlayButtonText == "再生")
            {
                settingInfo.PlaybackState = PlaybackState.Stopped;
            }
            else
            {
                settingInfo.PlaybackState = PlaybackState.Playing;
            }
            settingInfo.Volume = (int)Volume;

            jsonUtility.WriteJson(Constants.MainWindowSettingFileName, settingInfo);
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 幻想郷ラジオを停止
        /// </summary>
        public void StopRadio()
        {
            try
            {
                if (waveOutEvent != null)
                {
                    PlayButtonText = "再生";
                    //timer.Stop();
                    waveOutEvent.Pause();
                }
            }

            catch (Exception ex)
            {
                var errorMessage = "停止中にエラーが発生しました";
                MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
