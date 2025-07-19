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
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly WaveOutEvent waveOutEvent = new WaveOutEvent();

        /// <summary>
        /// 任意のファイルを読み込むクラス
        /// </summary>
        private readonly MediaFoundationReader reader = new MediaFoundationReader(Constants.STREAM_ENDPOINT);

        /// <summary>
        /// 設定ファイルの構造
        /// </summary>
        private SettingInfo settingInfo = new SettingInfo();

        /// <summary>
        /// HTTP/HTTPS通信を行うクラス
        /// </summary>
        private static readonly HttpClient client = new HttpClient();

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
        /// タイトルバーに表示する文字列
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

        private double volumeSlider;

        public double VolumeSlider
        {
            get => volumeSlider;
            set
            {
                volumeSlider = value;
                SetProperty();
            }
        }

        private string volumeText = "100";

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


        private int timeSliderMax = 0;

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

        private string nameLabelText = Constants.BLANK;

        public string NameLabelText
        {
            get => nameLabelText;
            set
            {
                nameLabelText = value;
                SetProperty();
            }
        }

        private string artistLabelText = Constants.BLANK;

        public string ArtistLabelText
        {
            get => artistLabelText;
            set
            {
                artistLabelText = value;
                SetProperty();
            }
        }

        private string albumNameLabelText = Constants.BLANK;

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

        public ICommand PlayButtonClickCommand { get; }

        public ICommand ArtWorkClickCommand { get; }

        public MainWindowViewModel(Window owner)
        {
            PlayButtonClickCommand = new RelayCommand(_ => PlayButtonClick(), _ => true);
            ArtWorkClickCommand = new RelayCommand(_ => ArtWorkClick(), _ => true);

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
                if (!Directory.Exists(Constants.SETTING_FOOTER))
                {
                    logger.Warn("設定ファイルのディレクトリが存在しません。");

                    // 設定ファイルのディレクトリを作成
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(Constants.SETTING_FOOTER);
                    logger.Info($"設定ファイルのディレクトリを「{directoryInfo.FullName}」へ作成しました。");
                }

                // 設定ファイルが存在しない場合
                if (!File.Exists(Constants.FORM_MAIN_SETTING_FILE_NAME))
                {
                    logger.Warn("設定ファイルが存在しません。");

                    // 設定ファイルを作成してデフォルトの設定値を書き込み
                    if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
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
                settingInfo = JsonUtility.ReadJson<SettingInfo>(Constants.FORM_MAIN_SETTING_FILE_NAME, null);

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
                waveOutEvent.Init(reader);
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
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.SONG_INFO_API);
                //SetStatusMessage("楽曲情報を取得しています...");
                logger.Info($"HTTPリクエストメッセージを作成しました。\n{request}");

                // HTTPリクエストを送信
                logger.Info($"HTTPリクエストメッセージを {request.RequestUri} へ送信します。");
                using HttpResponseMessage response = await client.SendAsync(request);
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
                            NameLabelText = Constants.BLANK;
                        }
                        NameLabelText = radioAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                        logger.Info($"楽曲名「{radioAPI.SONGINFO.TITLE}」を反映しました。");

                        if (string.IsNullOrWhiteSpace(radioAPI.SONGINFO.ARTIST))
                        {
                            ArtistLabelText = Constants.BLANK;
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
                            AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(Constants.ALBUM_ART_PLACEHOLDER);
                        }

                        // アルバムアートが定義されている場合
                        else
                        {
                            /* アルバムアート取得先の絶対パスと再生中の楽曲のアルバムアートのファイル名を合体して、
                            再生中の楽曲のアルバムアートのパスを特定 */
                            AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString($"{Constants.ALBUM_ART_PREFIX}{radioAPI.MISC.ALBUMART}");
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
                string link = $"{Constants.ALBUM_INFO_LINK}{radioAPI.SONGDATA.ALBUMID}/";

                if (MessageBox.Show(link, "アルバム情報をブラウザーで確認しますか？", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    using Process process = new Process();
                    process.StartInfo.FileName = link;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
            }

            else
            {
                _ = MessageBox.Show("このアルバムの情報はありません", "アルバム情報なし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
