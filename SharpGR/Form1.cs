using NAudio.Wave;
using NLog;
using SharpGR.FileIO;
using System.Net;
using System.Reflection;

namespace SharpGR
{
    /// <summary>
    /// メイン画面
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// 音声出力を行うクラス
        /// </summary>
        private readonly WaveOutEvent waveOut = new WaveOutEvent();

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
        /// 楽曲情報のレスポンスボディの構造
        /// </summary>
        private SongAPI? songAPI = new SongAPI();

        /// <summary>
        /// ログ出力を行うクラス
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 再生中の楽曲の総再生時間
        /// </summary>
        private int Duration;

        /// <summary>
        /// 現在の再生時間
        /// </summary>
        private int Played;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メイン画面ロード時
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Text = $"{assembly.GetName().Name} {assembly.GetName().Version}";
            ReadSettings();
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">表示するエラーメッセージ</param>
        private void SetErrorMessage(string message)
        {
            toolStripStatusLabel1.ForeColor = Color.Red;
            toolStripStatusLabel1.Text = message;
            logger.Info($"toolStripStatusLabel1に以下のメッセージを表示しました\n{message}");
        }

        /// <summary>
        /// ステータスメッセージを表示
        /// </summary>
        /// <param name="message">表示するステータスメッセージ</param>
        private void SetStatusMessage(string message)
        {
            toolStripStatusLabel1.ForeColor = Color.Green;
            toolStripStatusLabel1.Text = message;
        }

        /// <summary>
        /// メイン画面の設定ファイルの読み込み
        /// </summary>
        public async void ReadSettings()
        {
            try
            {
                // 設定ファイルのディレクトリが存在しない場合
                if (!Directory.Exists(Constants.SETTINGS_FOOTER))
                {
                    logger.Warn("設定ファイルのディレクトリが存在しません、設定ファイルのディレクトリを作成します。");

                    // 設定ファイルのディレクトリを作成
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(Constants.SETTINGS_FOOTER);
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
                        SetErrorMessage(errorMessage);
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
                    trackBarVol.Value = Convert.ToInt32(settingInfo.Volume);
                    numericUpDownVol.Text = settingInfo.Volume.ToString();
                    logger.Info($"音量を {settingInfo.Volume} %に設定しました。");

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOut.PlaybackState != PlaybackState.Playing)
                    {
                        logger.Info("幻想郷ラジオの再生を開始します。");
                        await StartRadioAsync();
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
                    SetErrorMessage(errorMessage);

                    // 音量を反映する
                    trackBarVol.Value = Convert.ToInt32(settingInfo.Volume);
                    numericUpDownVol.Text = settingInfo.Volume.ToString();

                    if (settingInfo.PlaybackState == PlaybackState.Playing && waveOut.PlaybackState != PlaybackState.Playing)
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
                SetErrorMessage(errorMessage);
                return;
            }
        }

        /// <summary>
        /// 再生/停止ボタン押下時
        /// </summary>
        private async void buttonPlay_Click(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
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
                waveOut.Init(reader);
                await GetSongInfoFromAPIAsync();
                timer1.Start();
                waveOut.Play();
                buttonPlay.Text = "停止";
            }

            catch (Exception ex)
            {
                string errorMessage = "再生中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(errorMessage);
            }
        }

        /// <summary>
        /// 1秒ごとに曲情報取得
        /// </summary>
        private async void timer1_Tick(object sender, EventArgs e)
        {
            await GetSongInfoFromAPIAsync();

            trackBarDuration.Maximum = Duration;

            if (Played <= Duration)
            {
                trackBarDuration.Value = Played;
            }
            else
            {
                Played = 0;
                trackBarDuration.Value = Played;
                Timelabel.Text = "--:-- / --:--";
            }
        }

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報を非同期で取得
        /// </summary>
        private async Task GetSongInfoFromAPIAsync()
        {
            try
            {
                logger.Info("幻想郷ラジオで再生中の楽曲情報を取得します。");
                SetErrorMessage(string.Empty);

                logger.Info("HTTPリクエストメッセージを作成します。");
                // HTTPリクエストを作成
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.SONG_INFO_API);
                //SetStatusMessage("楽曲情報を取得しています...");
                logger.Info($"HTTPリクエストメッセージを作成しました。\n{request}");

                logger.Info($"HTTPリクエストメッセージを {request.RequestUri} へ送信します。");
                // HTTPリクエストを送信
                using HttpResponseMessage response = await client.SendAsync(request);
                logger.Info($"HTTPリクエストメッセージに対するレスポンスメッセージが返されました。\n{response}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    logger.Info($"HTTPステータスコードは {response.StatusCode} です。");

                    logger.Info("レスポンスメッセージからレスポンスボディを読み込みます。");
                    // レスポンスボディを取得
                    string resBody = await response.Content.ReadAsStringAsync();
                    logger.Info($"レスポンスボディを読み込みました。\n{resBody}");

                    // レスポンスボディをデシリアライズ
                    logger.Info("レスポンスボディをC#で扱える形式へ変換します。");
                    songAPI = JsonUtility.ReadJson<SongAPI>(null, resBody);

                    if (songAPI != null)
                    {
                        logger.Info("レスポンスボディをC#で扱える形式へ変換しました。メイン画面の各要素へ楽曲情報として反映します。");
                        //SetStatusMessage("楽曲情報を取得しました");

                        // デシリアライズしたオブジェクトから楽曲情報を取得
                        Namelabel.Text = songAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                        logger.Info($"楽曲名「{songAPI.SONGINFO.TITLE}」を反映しました。");

                        Artistlabel.Text = songAPI.SONGINFO.ARTIST; // アーティスト名を設定
                        logger.Info($"アーティスト名「{songAPI.SONGINFO.ARTIST}」を反映しました。");

                        Albumlabel.Text = songAPI.SONGINFO.ALBUM;   // アルバム名を設定
                        logger.Info($"アルバム名「{songAPI.SONGINFO.ALBUM}」を反映しました。");

                        //Yearlabel.Text = songAPI.SONGINFO.YEAR;   // リリース年を設定
                        //Circlelabel.Text = songAPI.SONGINFO.CIRCLE;   // サークル名を設定
                        Duration = songAPI.SONGTIMES.DURATION;  // 楽曲の総再生時間を設定
                        logger.Info($"楽曲の長さ「{TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}」を反映しました。");

                        Played = songAPI.SONGTIMES.PLAYED;  // 現在の再生時間を設定
                        logger.Info($"現在の再生時間「{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")}」を反映しました。");

                        // 総再生時間と現在の再生時間を表示
                        Timelabel.Text = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";

                        // アルバムアートが未定義の場合
                        if (songAPI.MISC.ALBUMART == string.Empty || songAPI.MISC.ALBUMART == null)
                        {
                            // デフォルトのアルバムアートを表示
                            AlbumArtpictureBox.ImageLocation = Constants.ALBUM_ART_PLACEHOLDER;
                        }
                        // アルバムアートが定義されている場合
                        else
                        {
                            /* アルバムアート取得先の絶対パスと再生中の楽曲のアルバムアートのファイル名を合体して、
                            再生中の楽曲のアルバムアートのパスを特定 */
                            AlbumArtpictureBox.ImageLocation = $"{Constants.ALBUM_ART_PREFIX}{songAPI.MISC.ALBUMART}";
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
                    throw new HttpRequestException($"ステータスコードが {response.StatusCode} でした。");
                }
            }

            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                _ = MessageBox.Show(errorMessage, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(errorMessage);
            }
        }

        /// <summary>
        /// 幻想郷ラジオを停止
        /// </summary>
        private void StopRadio()
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    timer1.Stop();
                    buttonPlay.Text = "再生";
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "停止中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(errorMessage);
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            SetErrorMessage(string.Empty);
        }

        private void 操作方法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHelp formHelp = new FormHelp();
            formHelp.Show();
        }

        private void sharpGRについてToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.Show();
        }

        /// <summary>
        /// 音量変更時(トラックバー)
        /// </summary>
        private void trackBarVol_Scroll(object sender, EventArgs e)
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.Volume = trackBarVol.Value / 100f;
                    numericUpDownVol.Text = trackBarVol.Value.ToString();
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "音量の変更中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(errorMessage);
            }
        }

        /// <summary>
        /// 音量変更時(スピンボックス)
        /// </summary>
        private void numericUpDownVol_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.Volume = trackBarVol.Value / 100f;
                    trackBarVol.Value = Convert.ToInt32(numericUpDownVol.Value);
                }
            }

            catch (Exception ex)
            {
                string errorMessage = "音量の変更中にエラーが発生しました";
                _ = MessageBox.Show(errorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(errorMessage);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 設定ファイルへ保存
        /// </summary>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (buttonPlay.Text)
            {
                case "再生":
                    settingInfo.PlaybackState = PlaybackState.Stopped;
                    settingInfo.Volume = Convert.ToInt32(numericUpDownVol.Text);
                    break;
                case "停止":
                    settingInfo.PlaybackState = PlaybackState.Playing;
                    settingInfo.Volume = Convert.ToInt32(numericUpDownVol.Text);
                    break;
                default:
                    break;
            }

            if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
            {
                _ = MessageBox.Show("設定を保存出来ませんでした", "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}