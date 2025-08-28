using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using NAudio.Wave;
using NLog;
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

        private readonly WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// 任意のファイルを読み込むクラス
        /// </summary>
        private readonly MediaFoundationReader mediaFoundationReader = new MediaFoundationReader(Constants.StreamEndpoint);

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
        private int Duration;

        /// <summary>
        /// 現在の再生時間
        /// </summary>
        private int Played;

#nullable enable
        /// <summary>
        /// 幻想郷ラジオのAPI
        /// </summary>
        private RadioAPI? radioAPI = new RadioAPI();
#nullable disable

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
        /// 音量テキストボックスの初期の文字列
        /// </summary>
        private double volume = 100;

        /// <summary>
        /// 前回の音量値の保存用
        /// </summary>
        private double previousVolume;

        /// <summary>
        /// 音量テキストボックスの新しい文字列
        /// </summary>
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                if (double.TryParse(value.ToString(), out double parsedValue) && IsValidRange(parsedValue))
                {
                    previousVolume = parsedValue;
                    SetProperty();
                    UpdateNAudioVolume();
                }
                else
                {
                    volume = previousVolume;
                    SetProperty();
                    MessageBox.Show("無効な値です");
                }
            }
        }

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


        private int timeSliderMax = 100;

        public int TimeSliderMax
        {
            get => timeSliderMax;
            set
            {
                timeSliderMax = value;
                SetProperty();
            }
        }

        private int timeSliderValue = 0;

        public int TimeSliderValue
        {
            get => timeSliderValue;
            set
            {
                timeSliderValue = value;
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

        private DispatcherTimer dispatcherTimer;

        private string lastTitle = string.Empty;

        private JsonUtility jsonUtility;

        #endregion

        #region Command

        /// <summary>
        /// 再生ボタンクリック時のイベント
        /// </summary>
        public ICommand PlayButtonClickCommand { get; }

        /// <summary>
        /// アルバムアートクリック時のイベント
        /// </summary>
        public ICommand ArtWorkClickCommand { get; }

        /// <summary>
        /// 画面を閉じるコマンド
        /// </summary>
        public ICommand CloseCommand { get; }

        #endregion

        public MainWindowViewModel()
        {
            jsonUtility = new JsonUtility();
            // 再生ボタンクリック時のイベントにメソッドをバインド
            PlayButtonClickCommand = new RelayCommand(_ => PlayButtonClick(), _ => true);

            // アルバムアートクリック時のイベントにメソッドをバインド
            ArtWorkClickCommand = new RelayCommand(_ => ArtWorkClick(), _ => true);

            CloseCommand = new RelayCommand(_ => CloseWindow(), _ => true);

            // タイトルバーに表示する文字列を設定
            AppTitle = $"{Assembly.GetExecutingAssembly().GetName().Name} Ver.{Assembly.GetExecutingAssembly().GetName().Version}";

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += (s, e) =>
            {
                UpdateCurrentTime();
            };
            dispatcherTimer.Start();

            _ = ReadSettingsAsync();
        }

        /// <summary>
        /// メイン画面の設定ファイルの読み込み
        /// </summary>
        public async Task ReadSettingsAsync()
        {
            try
            {
                // 設定ファイルのディレクトリが存在しない場合
                if (!Directory.Exists(Constants.MainWindowSettingFileName))
                {
                    logger.Warn("設定ファイルのディレクトリが存在しません。");

                    // 設定ファイルのディレクトリを作成
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(Constants.SettingFooter);
                    logger.Info($"設定ファイルのディレクトリを「{directoryInfo.FullName}」へ作成しました。");
                }

                // 設定ファイルが存在しない場合
                if (!File.Exists(Constants.MainWindowSettingFileName))
                {
                    logger.Warn("設定ファイルが存在しません。");

                    // 設定ファイルを作成してデフォルトの設定値を書き込み
                    if (!jsonUtility.WriteJson(Constants.MainWindowSettingFileName, settingInfo))
                    {
                        string errorMessage = "設定ファイルの作成、またはデフォルトの設定値の書き込みに失敗しました。";
                        logger.Warn(errorMessage);
                        _ = MessageBox.Show(errorMessage, "設定ファイルへ書き込めませんでした", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else
                    {
                        logger.Info("設定ファイルにデフォルトの設定値を書き込みました。");
                    }
                }

                logger.Info("設定ファイルを読み込みます。");
                settingInfo = null;
                settingInfo = jsonUtility.ReadSettingJson(Constants.MainWindowSettingFileName);

                // 設定ファイルを読み込めた時
                if (settingInfo != null)
                {
                    logger.Info($"設定ファイルを読み込みました。\n再生状態は {settingInfo.PlaybackState} です。\n音量は {settingInfo.Volume} %です。");

                    // 音量を反映する
                    int volume = settingInfo.Volume;
                    Volume = volume;
                    waveOutEvent.Volume = (float)(volume / 100.0);
                    logger.Info($"音量を {volume} %に設定しました。");
                    //timer.Tick += Timer_Tick;
                    //timer.Start();
                    await GetSongInfoFromAPIAsync();

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOutEvent.PlaybackState != PlaybackState.Playing)
                    {
                        logger.Info("幻想郷ラジオの再生を開始します。");
                        StartRadio();
                    }

                    else
                    {
                        StopRadio();
                    }
                }

                // 設定ファイルを読み込めなかった時
                else
                {
                    SettingInfo settingInfo = new SettingInfo();
                    string errorMessage = "設定を読み込めませんでした、デフォルトの設定値を使用します。";
                    _ = MessageBox.Show(errorMessage, "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // 音量を反映する
                    int volume = settingInfo.Volume;
                    Volume = volume;
                    waveOutEvent.Volume = volume;

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOutEvent.PlaybackState != PlaybackState.Playing)
                    {
                        StartRadio();
                    }

                    else
                    {
                        StopRadio();
                    }
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "設定の読み込み中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void StartRadio()
        {
            try
            {
                PlayButtonText = "停止";
                waveOutEvent.Init(mediaFoundationReader);
                waveOutEvent.Play();
            }

            catch (Exception ex)
            {
                string errorMessage = "再生中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// ユーザーが入力した音量の範囲チェック
        /// </summary>
        /// <param name="value">ユーザーが入力した音量</param>
        /// <returns>0～100の範囲内なら true、範囲外なら false</returns>
        private bool IsValidRange(double value)
        {
            return value >= 0 && value <= 100;
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
            while (true)
            {
                try
                {
                    logger.Info("幻想郷ラジオで再生中の楽曲情報を取得します。");

                    // HTTPリクエストメッセージを作成
                    logger.Info("HTTPリクエストメッセージを作成します。");
                    using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.RadioAPIURL);
                    //SetStatusMessage("楽曲情報を取得しています...");
                    logger.Info($"HTTPリクエストメッセージを作成しました。\n{request}");

                    // HTTPリクエストを送信
                    logger.Info($"HTTPリクエストメッセージを {request.RequestUri} へ送信します。");
                    using HttpResponseMessage response = await httpClient.SendAsync(request);
                    logger.Info($"HTTPリクエストメッセージに対して以下のレスポンスメッセージが返されました。\n{response}");

                    if (response.IsSuccessStatusCode)
                    {
                        logger.Info($"HTTPステータスコードは {response.StatusCode} です。");

                        logger.Info("レスポンスメッセージからレスポンスボディを読み込みます。");

                        // レスポンスボディを読み込む
                        string resBody = await response.Content.ReadAsStringAsync();
                        logger.Info($"以下のレスポンスボディを読み込みました。\n{resBody}");

                        jsonUtility.ParseAndSetData(resBody, this);

                        string newTitle = JsonUtility.GetTitleFromResponse(resBody);

                        if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != NameLabelText)
                        {
                            NameLabelText = newTitle;
                            jsonUtility.ParseAndSetData(resBody, this);
                        }

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
                    string errorMessage = ex.Message;
                    _ = MessageBox.Show(errorMessage, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }

        /// <summary>
        /// 現在の再生時間を更新（タイマー表示）
        /// </summary>
        private void UpdateCurrentTime()
        {
            // 総再生時間と現在の再生時間を表示
            TimeLabelText = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";
        }

        private void ArtWorkClick()
        {
            if (!string.IsNullOrWhiteSpace(radioAPI.SONGDATA.ALBUMID.ToString()))
            {
                string link = $"{Constants.RadioAPIURL}{radioAPI.SONGDATA.ALBUMID}/";

                if (MessageBox.Show(link, "アルバム情報をブラウザーで確認しますか？", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    using Process process = new Process();
                    process.StartInfo.FileName = link;
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
        private void StopRadio()
        {
            try
            {
                if (waveOutEvent != null)
                {
                    PlayButtonText = "再生";
                    //timer.Stop();
                    waveOutEvent.Stop();
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "停止中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
