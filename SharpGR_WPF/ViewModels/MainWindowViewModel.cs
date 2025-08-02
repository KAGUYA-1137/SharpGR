using NAudio.Wave;
using NLog;
using SharpGR_WPF.FileIO;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SharpGR_WPF.ViewModels
{
    /// <summary>
    /// MainWindowのビューモデル
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region Constants

        /// <summary>
        /// 設定ファイルのディレクトリ
        /// </summary>
        private const string SETTING_FOOTER = "Setting\\";

        /// <summary>
        /// メイン画面の設定が記載されたファイル名
        /// </summary>
        private const string FORM_MAIN_SETTING_FILE_NAME = $"{SETTING_FOOTER}Form1.json";

        /// <summary>
        /// 幻想郷ラジオのストリームエンドポイント
        /// </summary>
        private const string STREAM_ENDPOINT = "https://stream.gensokyoradio.net/3";

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報の取得先
        /// </summary>
        private const string SONG_INFO_API = "https://gensokyoradio.net/api/station/playing/";

        /// <summary>
        /// 幻想郷ラジオのアルバムアート取得先の絶対パス
        /// </summary>
        private const string ALBUM_ART_PREFIX = "https://gensokyoradio.net/images/albums/500/";

        /// <summary>
        /// 幻想郷ラジオのデフォルトのアルバムアートのパス
        /// </summary>
        private const string ALBUM_ART_PLACEHOLDER = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";

        /// <summary>
        /// 幻想郷ラジオのアルバム情報のURL
        /// </summary>
        private const string ALBUM_INFO_LINK = "https://gensokyoradio.net/music/album/";

        private const string BLANK = "----";

        #endregion

        private readonly WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// 任意のファイルを読み込むクラス
        /// </summary>
        private readonly MediaFoundationReader mediaFoundationReader = new MediaFoundationReader(STREAM_ENDPOINT);

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
        /// 音量スライダーの初期の値
        /// </summary>
        private double volumeSlider = 100;

        /// <summary>
        /// 音量スライダーの新しい値
        /// </summary>
        public double VolumeSlider
        {
            get => volumeSlider;
            set
            {
                volumeSlider = value;
                SetProperty();
            }
        }

        /// <summary>
        /// 音量テキストボックスの初期の文字列
        /// </summary>
        private string volumeText = "100";

        /// <summary>
        /// 音量テキストボックスの新しい文字列
        /// </summary>
        public string VolumeText
        {
            get => volumeText;
            set
            {
                volumeText = value;
                SetProperty();
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

        private string timeLabelText = "--:-- / --:--";

        public string TimeLabelText
        {
            get => timeLabelText;
            set
            {
                timeLabelText = value;
                SetProperty();
            }
        }

        private string nameLabelText = BLANK;

        public string NameLabelText
        {
            get => nameLabelText;
            set
            {
                nameLabelText = value;
                SetProperty();
            }
        }

        private string artistLabelText = BLANK;

        public string ArtistLabelText
        {
            get => artistLabelText;
            set
            {
                artistLabelText = value;
                SetProperty();
            }
        }

        private string albumNameLabelText = BLANK;

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

        public MainWindowViewModel(Window owner)
        {
            // 再生ボタンクリック時のイベントにメソッドをバインド
            PlayButtonClickCommand = new RelayCommand(_ => PlayButtonClick(), _ => true);

            // アルバムアートクリック時のイベントにメソッドをバインド
            ArtWorkClickCommand = new RelayCommand(_ => ArtWorkClick(), _ => true);

            CloseCommand = new RelayCommand(_ => CloseWindow(), _ => true);

            // タイトルバーに表示する文字列を設定
            AppTitle = $"{Assembly.GetExecutingAssembly().GetName().Name} Ver.{Assembly.GetExecutingAssembly().GetName().Version}";

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
                if (!Directory.Exists(SETTING_FOOTER))
                {
                    logger.Warn("設定ファイルのディレクトリが存在しません。");

                    // 設定ファイルのディレクトリを作成
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(SETTING_FOOTER);
                    logger.Info($"設定ファイルのディレクトリを「{directoryInfo.FullName}」へ作成しました。");
                }

                // 設定ファイルが存在しない場合
                if (!File.Exists(FORM_MAIN_SETTING_FILE_NAME))
                {
                    logger.Warn("設定ファイルが存在しません。");

                    // 設定ファイルを作成してデフォルトの設定値を書き込み
                    if (!JsonUtility.WriteJson(FORM_MAIN_SETTING_FILE_NAME, settingInfo))
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
                settingInfo = JsonUtility.ReadJson<SettingInfo>(FORM_MAIN_SETTING_FILE_NAME, null);

                // 設定ファイルを読み込めた時
                if (settingInfo != null)
                {
                    logger.Info($"設定ファイルを読み込みました。\n再生状態は {settingInfo.PlaybackState} です。\n音量は {settingInfo.Volume} %です。");

                    // 音量を反映する
                    //VolumeSlider.Value = Convert.ToInt32(settingInfo.Volume);
                    //VolumeTextBox.Text = settingInfo.Volume.ToString();
                    VolumeSlider = settingInfo.Volume;
                    VolumeText = settingInfo.Volume.ToString();
                    logger.Info($"音量を {settingInfo.Volume} %に設定しました。");

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOutEvent.PlaybackState != PlaybackState.Playing)
                    {
                        logger.Info("幻想郷ラジオの再生を開始します。");
                        await StartRadioAsync();
                        timer.Tick += Timer_Tick;
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
                    //VolumeSlider.Value = Convert.ToInt32(settingInfo.Volume);
                    //VolumeTextBox.Text = settingInfo.Volume.ToString();
                    VolumeSlider = settingInfo.Volume;
                    VolumeText = settingInfo.Volume.ToString();

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOutEvent.PlaybackState != PlaybackState.Playing)
                    {
                        await StartRadioAsync();
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

        private async void PlayButtonClick()
        {
            if (waveOutEvent.PlaybackState != PlaybackState.Playing)
            {
                await StartRadioAsync();
            }

            else
            {
                StopRadio();
            }
        }

        /// <summary>
        /// 幻想郷ラジオを再生
        /// </summary>
        private async Task StartRadioAsync()
        {
            try
            {
                PlayButtonText = "停止";
                waveOutEvent.Init(mediaFoundationReader);
                await GetSongInfoFromAPIAsync();
                timer.Start();
                waveOutEvent.Play();
            }

            catch (Exception ex)
            {
                string errorMessage = "再生中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 1秒ごとに曲情報取得
        /// </summary>
        private async void Timer_Tick(object sender, EventArgs e)
        {
            await GetSongInfoFromAPIAsync();

            TimeSliderMax = Duration;

            if (Played <= Duration)
            {
                TimeSliderValue = Played;
            }

            else
            {
                Played = 0;
                TimeSliderValue = Played;
                TimeLabelText = "--:-- / --:--";
            }
        }

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報を非同期で取得して画面へ反映。
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        private async Task GetSongInfoFromAPIAsync()
        {
            try
            {
                logger.Info("幻想郷ラジオで再生中の楽曲情報を取得します。");

                // HTTPリクエストメッセージを作成
                logger.Info("HTTPリクエストメッセージを作成します。");
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, SONG_INFO_API);
                //SetStatusMessage("楽曲情報を取得しています...");
                logger.Info($"HTTPリクエストメッセージを作成しました。\n{request}");

                // HTTPリクエストを送信
                logger.Info($"HTTPリクエストメッセージを {request.RequestUri} へ送信します。");
                using HttpResponseMessage response = await httpClient.SendAsync(request);
                logger.Info($"HTTPリクエストメッセージに対して以下のレスポンスメッセージが返されました。\n{response}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    logger.Info($"HTTPステータスコードは {response.StatusCode} です。");

                    logger.Info("レスポンスメッセージからレスポンスボディを読み込みます。");

                    // レスポンスボディを読み込む
                    string resBody = await response.Content.ReadAsStringAsync();
                    logger.Info($"以下のレスポンスボディを読み込みました。\n{resBody}");

                    // レスポンスボディをデシリアライズ
                    radioAPI = JsonUtility.ReadJson<RadioAPI>(null, resBody);

                    if (radioAPI != null)
                    {
                        logger.Info("メイン画面の各コンポーネントへ楽曲情報を反映します。");
                        //SetStatusMessage("楽曲情報を取得しました");

                        // デシリアライズしたオブジェクトから楽曲情報を取得
                        if (string.IsNullOrWhiteSpace(radioAPI.SONGINFO.TITLE))
                        {
                            NameLabelText = BLANK;
                        }
                        NameLabelText = radioAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                        logger.Info($"楽曲名「{radioAPI.SONGINFO.TITLE}」を反映しました。");

                        if (string.IsNullOrWhiteSpace(radioAPI.SONGINFO.ARTIST))
                        {
                            ArtistLabelText = BLANK;
                        }
                        ArtistLabelText = radioAPI.SONGINFO.ARTIST; // アーティスト名を設定
                        logger.Info($"アーティスト名「{radioAPI.SONGINFO.ARTIST}」を反映しました。");

                        AlbumNameLabelText = radioAPI.SONGINFO.ALBUM;   // アルバム名を設定
                        logger.Info($"アルバム名「{radioAPI.SONGINFO.ALBUM}」を反映しました。");

                        Duration = radioAPI.SONGTIMES.DURATION;  // 楽曲の総再生時間を設定
                        logger.Info($"楽曲の長さ「{TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}」を反映しました。");

                        Played = radioAPI.SONGTIMES.PLAYED;  // 現在の再生時間を設定
                        logger.Info($"現在の再生時間「{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")}」を反映しました。");

                        // 総再生時間と現在の再生時間を表示
                        TimeLabelText = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";

                        // アルバムアートが未定義の場合
                        if (string.IsNullOrWhiteSpace(radioAPI.MISC.ALBUMART))
                        {
                            // デフォルトのアルバムアートを表示
                            AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(ALBUM_ART_PLACEHOLDER);
                        }

                        // アルバムアートが定義されている場合
                        else
                        {
                            /* アルバムアート取得先の絶対パスと再生中の楽曲のアルバムアートのファイル名を合体して、
                            再生中の楽曲のアルバムアートのパスを特定 */
                            AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString($"{ALBUM_ART_PREFIX}{radioAPI.MISC.ALBUMART}");
                        }
                    }

                    // 楽曲情報が空の場合
                    else
                    {
                        throw new NullReferenceException("楽曲情報を取得できませんでした。");
                    }
                }

                // ステータスコードが200 OK以外の場合
                else
                {
                    logger.Error($"HTTPリクエストを送信しましたが、ステータスコードが {response.StatusCode} でした。");
                    throw new HttpRequestException($"HTTPリクエストに失敗しました。\nステータスコード： {response.StatusCode}");
                }
            }

            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                _ = MessageBox.Show(errorMessage, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ArtWorkClick()
        {
            if (!string.IsNullOrWhiteSpace(radioAPI.SONGDATA.ALBUMID.ToString()))
            {
                string link = $"{ALBUM_INFO_LINK}{radioAPI.SONGDATA.ALBUMID}/";

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
