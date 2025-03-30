using System.Reflection;
using NAudio.Wave;
using Newtonsoft.Json;
using SharpGR.FileIO;

namespace SharpGR
{
    /// <summary>
    /// Form1のクラス
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly WaveOutEvent waveOut = new WaveOutEvent();
        private readonly MediaFoundationReader reader = new MediaFoundationReader(Constants.STREAM_ENDPOINT);
        private string ErrorMessage = string.Empty;
        private SettingInfo settingInfo = new SettingInfo();
        private static readonly HttpClient _httpClient = new HttpClient();
        private SongAPI? songAPI = new SongAPI();

        /// <summary>
        /// 総再生時間
        /// </summary>
        /// 
        private int Duration;

        /// <summary>
        /// 現在の再生時間
        /// </summary>
        private int Played;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メイン画面ロード時
        /// </summary>
        private async void Form1_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Text = $"{assembly.GetName().Name} {assembly.GetName().Version}";
            await ReadSettings();
        }

        /// <summary>
        /// エラーメッセージを表示
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private void SetErrorMessage(string message)
        {
            toolStripStatusLabel1.ForeColor = Color.Red;
            toolStripStatusLabel1.Text = message;
        }

        /// <summary>
        /// 設定読み込み
        /// </summary>
        public async Task ReadSettings()
        {
            // 設定ファイルが存在しない場合
            if (!File.Exists(Constants.FORM_MAIN_SETTING_FILE_NAME))
            {
                // 設定ファイルを作成してデフォルトの設定値を書き込み
                if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
                {
                    ErrorMessage = "デフォルトの設定値を設定ファイルへ書き込めませんでした";
                    MessageBox.Show(ErrorMessage, "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetErrorMessage(ErrorMessage);
                }
            }

            // 設定ファイルを読み込めた時
            if (JsonUtility.ReadJson(Constants.FORM_MAIN_SETTING_FILE_NAME) != null)
            {
                settingInfo = JsonUtility.ReadJson(Constants.FORM_MAIN_SETTING_FILE_NAME);

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

            // 設定ファイルを読み込めなかった時
            else
            {
                ErrorMessage = "設定を読み込めませんでした、デフォルトの設定値を使用します。";
                MessageBox.Show(ErrorMessage, "エラーが発生しました", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);

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
                ErrorMessage = "再生中にエラーが発生しました";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);
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
                // HTTPリクエストを作成
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, Constants.MUSIC_INFO_JSON);
                // HTTPリクエストを送信
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                // レスポンスボディを取得
                string resBody = await response.Content.ReadAsStringAsync();

                // レスポンスボディをデシリアライズ
                songAPI = JsonConvert.DeserializeObject<SongAPI>(resBody);

                if (songAPI != null)
                {
                    // デシリアライズしたオブジェクトから楽曲情報を取得
                    Namelabel.Text = songAPI.SONGINFO.TITLE;    // 楽曲のタイトルを設定
                    Artistlabel.Text = songAPI.SONGINFO.ARTIST; // アーティスト名を設定
                    Albumlabel.Text = songAPI.SONGINFO.ALBUM;   // アルバム名を設定
                    //Yearlabel.Text = songAPI.SONGINFO.YEAR;   // リリース年を設定
                    //Circlelabel.Text = songAPI.SONGINFO.CIRCLE;   // サークル名を設定
                    Duration = songAPI.SONGTIMES.DURATION;  // 楽曲の総再生時間を設定
                    Played = songAPI.SONGTIMES.PLAYED;  // 現在の再生時間を設定
                    Timelabel.Text = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration)
                        .ToString(@"m\:ss")}";    // 総再生時間と現在の再生時間を表示

                    // アルバムアートが空のとき
                    if (songAPI.MISC.ALBUMART == string.Empty)
                    {
                        // デフォルトのアルバムアートを表示
                        AlbumArtpictureBox.ImageLocation = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";
                    }
                    else
                    {
                        /* アルバムアート取得先のプレフィックスと再生中の楽曲のアルバムアートのファイル名を合体して、
                        再生中の楽曲のアルバムアートのパスを特定 */
                        AlbumArtpictureBox.ImageLocation = $"{Constants.ALBUM_ART_PREFIX}{songAPI.MISC.ALBUMART}";
                    }
                }
                else
                {
                    throw new NullReferenceException("楽曲情報を取得できませんでした。");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageBox.Show(ErrorMessage, "エラーが発生しました。", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);
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
                ErrorMessage = "停止中にエラーが発生しました";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);
            }
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
                ErrorMessage = "音量の変更中にエラーが発生しました";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);
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
                ErrorMessage = "音量の変更中にエラーが発生しました";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetErrorMessage(ErrorMessage);
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

            try
            {
                JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo);
            }

            catch (Exception ex)
            {
                MessageBox.Show("設定の書き込み中にエラーが発生しました", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}