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

namespace SharpGR_WPF
{
    /// <summary>
    /// Form1.xaml の相互作用ロジック
    /// </summary>
    public partial class Form1 : Window
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
        /// 楽曲情報のレスポンスボディの構造
        /// </summary>
        private SongAPI? songAPI = new SongAPI();
#nullable disable

        /// <summary>
        /// ログ出力を行うクラス
        /// </summary>
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メイン画面ロード時
        /// </summary>
        private async void Form1_Load(object sender, RoutedEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Title = $"{assembly.GetName().Name} Ver.{assembly.GetName().Version}";
            await ReadSettingsAsync();
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
                    logger.Warn("設定ファイルのディレクトリが存在しません、設定ファイルのディレクトリを作成します。");

                    // 設定ファイルのディレクトリを作成
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(Constants.SETTING_FOOTER);
                    logger.Info($"設定ファイルのディレクトリを「{directoryInfo.FullName}」へ作成しました。");
                }

                // 設定ファイルが存在しない場合
                if (!File.Exists(Constants.FORM_MAIN_SETTING_FILE_NAME))
                {
                    logger.Warn("設定ファイルが存在しません、設定ファイルを作成しデフォルトの設定値を書き込みます。");

                    // 設定ファイルを作成してデフォルトの設定値を書き込み
                    if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
                    {
                        string errorMessage = "設定ファイルの作成またはデフォルトの設定値を書き込めませんでした。\n起動は行いますが設定ファイルは未作成です。";
                        logger.Warn(errorMessage);
                        _ = MessageBox.Show(errorMessage, "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else
                    {
                        logger.Info("設定ファイルにデフォルトの設定値を書き込みました。");
                    }
                }

                logger.Info("設定ファイルを読み込みます。");
                settingInfo = JsonUtility.ReadJson<SettingInfo>(Constants.FORM_MAIN_SETTING_FILE_NAME, null);

                // 設定ファイルを読み込めた時
                if (settingInfo != null)
                {
                    logger.Info($"設定ファイルを読み込みました。\n再生状態は {settingInfo.PlaybackState} です。\n音量は {settingInfo.Volume} %です。");

                    // 音量を反映する
                    VolumeSlider.Value = Convert.ToInt32(settingInfo.Volume);
                    VolumeTextBox.Text = settingInfo.Volume.ToString();
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
                    VolumeSlider.Value = Convert.ToInt32(settingInfo.Volume);
                    VolumeTextBox.Text = settingInfo.Volume.ToString();

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

        private async void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (waveOutEvent != null && waveOutEvent.PlaybackState == PlaybackState.Playing)
            {
                StopRadio();
            }

            else
            {
                await StartRadioAsync();
            }
        }

        /// <summary>
        /// 幻想郷ラジオを再生
        /// </summary>
        private async Task StartRadioAsync()
        {
            try
            {
                waveOutEvent.Init(reader);
                await GetSongInfoFromAPIAsync();
                timer.Start();
                waveOutEvent.Play();
                PlayButton.Content = "停止";
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

            TimeSlider.Maximum = Duration;

            if (Played <= Duration)
            {
                TimeSlider.Value = Played;
            }
            else
            {
                Played = 0;
                TimeSlider.Value = Played;
                TimeLabel.Content = "--:-- / --:--";
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
                    songAPI = JsonUtility.ReadJson<SongAPI>(null, resBody);

                    if (songAPI != null)
                    {
                        logger.Info("メイン画面の各コンポーネントへ楽曲情報を反映します。");
                        //SetStatusMessage("楽曲情報を取得しました");

                        // デシリアライズしたオブジェクトから楽曲情報を取得
                        if (string.IsNullOrWhiteSpace(songAPI.SONGINFO.TITLE))
                        {
                            NameLabel.Content = Constants.NO_TITLE;
                        }
                        NameLabel.Content = songAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                        logger.Info($"楽曲名「{songAPI.SONGINFO.TITLE}」を反映しました。");

                        if (string.IsNullOrWhiteSpace(songAPI.SONGINFO.ARTIST))
                        {
                            ArtistLabel.Content = Constants.NO_ARTIST;
                        }
                        ArtistLabel.Content = songAPI.SONGINFO.ARTIST; // アーティスト名を設定
                        logger.Info($"アーティスト名「{songAPI.SONGINFO.ARTIST}」を反映しました。");

                        AlbumNameLabel.Content = songAPI.SONGINFO.ALBUM;   // アルバム名を設定
                        logger.Info($"アルバム名「{songAPI.SONGINFO.ALBUM}」を反映しました。");

                        //Yearlabel.Text = songAPI.SONGINFO.YEAR;   // リリース年を設定
                        //Circlelabel.Text = songAPI.SONGINFO.CIRCLE;   // サークル名を設定

                        Duration = songAPI.SONGTIMES.DURATION;  // 楽曲の総再生時間を設定
                        logger.Info($"楽曲の長さ「{TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}」を反映しました。");

                        Played = songAPI.SONGTIMES.PLAYED;  // 現在の再生時間を設定
                        logger.Info($"現在の再生時間「{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")}」を反映しました。");

                        // 総再生時間と現在の再生時間を表示
                        TimeLabel.Content = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";

                        ListenerNumLabel.Content = songAPI.SERVERINFO.LISTENERS;
                        RatingNumLabel.Content = songAPI.SONGDATA.RATING;

                        // アルバムアートが未定義の場合
                        if (string.IsNullOrWhiteSpace(songAPI.MISC.ALBUMART))
                        {
                            // デフォルトのアルバムアートを表示
                            AlbumArtImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(Constants.ALBUM_ART_PLACEHOLDER);
                        }

                        // アルバムアートが定義されている場合
                        else
                        {
                            /* アルバムアート取得先の絶対パスと再生中の楽曲のアルバムアートのファイル名を合体して、
                            再生中の楽曲のアルバムアートのパスを特定 */
                            AlbumArtImage.Source = (ImageSource)new ImageSourceConverter().ConvertFromString($"{Constants.ALBUM_ART_PREFIX}{songAPI.MISC.ALBUMART}");
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

        /// <summary>
        /// 幻想郷ラジオを停止
        /// </summary>
        private void StopRadio()
        {
            try
            {
                logger.Info($"{Assembly.GetExecutingAssembly().GetName().Name}を終了します。");

                if (waveOutEvent != null)
                {
                    timer.Stop();
                    waveOutEvent.Stop();
                    PlayButton.Content = "再生";
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "停止中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void 操作方法ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    FormHelp formHelp = new FormHelp();
        //    formHelp.Show();
        //}

        //private void sharpGRについてToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    FormAbout formAbout = new FormAbout();
        //    formAbout.Show();
        //}

        /// <summary>
        /// 音量変更時(トラックバー)
        /// </summary>
        private void Volume_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if (waveOutEvent != null)
                {
                    waveOutEvent.Volume = (float)(VolumeSlider.Value / 100f);

                    if (VolumeTextBox != null)
                    {
                        //VolumeTextBox.Text = waveOutEvent.Volume.ToString();
                        VolumeTextBox.Text = Math.Floor(VolumeSlider.Value).ToString();
                    }

                    else
                    {
                        VolumeTextBox = new System.Windows.Controls.TextBox();
                    }
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "音量の変更中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 設定ファイルへ保存
        /// </summary>
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (PlayButton.Content)
            {
                case "再生":
                    settingInfo.PlaybackState = PlaybackState.Stopped;
                    settingInfo.Volume = Convert.ToInt32(VolumeTextBox.Text);
                    break;
                case "停止":
                    settingInfo.PlaybackState = PlaybackState.Playing;
                    settingInfo.Volume = Convert.ToInt32(VolumeTextBox.Text);
                    break;
                default:
                    break;
            }

            if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
            {
                _ = MessageBox.Show("設定を保存出来ませんでした", "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }

        private void AlbumArt_Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(songAPI.SONGDATA.ALBUMID.ToString()))
            {
                string link = $"{Constants.ALBUM_INFO_LINK}{songAPI.SONGDATA.ALBUMID}/";

                if (MessageBox.Show(link, "アルバム情報をブラウザーで確認しますか？", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    using Process process = new Process();
                    process.StartInfo.FileName = link;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
            }

            else
            {
                _ = MessageBox.Show("このアルバムの情報はありません", "情報なし", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Volume_TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (waveOutEvent != null)
                {
                    switch (e.Key)
                    {
                        case Key.Enter:
                            waveOutEvent.Volume = float.Parse(VolumeTextBox.Text) / 100f;
                            VolumeSlider.Value = Math.Floor(VolumeSlider.Value);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    throw new NullReferenceException($"{waveOutEvent}がnullです");
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "音量の変更中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
