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
        private readonly MediaFoundationReader reader = new MediaFoundationReader("https://stream.gensokyoradio.net/3");
        private SettingInfo settingInfo = new SettingInfo();
        private SongAPI songAPI = new SongAPI();
        private string ErrorMessage = string.Empty;
        private string urlInfoJson = "https://gensokyoradio.net/api/station/playing/";
        private string urlAlbumArt = "https://gensokyoradio.net/images/albums/500/";
        private int Duration;
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
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = ErrorMessage;
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
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;

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
                await getSongFromAPIAsync();
                timer1.Start();
                waveOut.Play();
                buttonPlay.Text = "停止";
            }

            catch (Exception ex)
            {
                ErrorMessage = "再生中にエラーが発生しました";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            await getSongFromAPIAsync();

            trackBarDuration.Maximum = Duration;

            if (Played < Duration)
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

        private async Task getSongFromAPIAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, urlInfoJson);
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string resBody = await response.Content.ReadAsStringAsync();

                songAPI = JsonConvert.DeserializeObject<SongAPI>(resBody);

                Namelabel.Text = songAPI.SONGINFO.TITLE;
                Artistlabel.Text = songAPI.SONGINFO.ARTIST;
                Albumlabel.Text = songAPI.SONGINFO.ALBUM;
                Yearlabel.Text = songAPI.SONGINFO.YEAR;
                Circlelabel.Text = songAPI.SONGINFO.CIRCLE;
                Duration = songAPI.SONGTIMES.DURATION;
                Played = songAPI.SONGTIMES.PLAYED;
                Timelabel.Text = $"{TimeSpan.FromSeconds(Played).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(Duration).ToString(@"m\:ss")}";

                // アルバムアートが空のとき(インターミッション再生時とか)
                if (songAPI.MISC.ALBUMART == string.Empty)
                {
                    AlbumArtpictureBox.ImageLocation = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";
                }
                else
                {
                    AlbumArtpictureBox.ImageLocation = $"{urlAlbumArt}{songAPI.MISC.ALBUMART}";
                }
            }
            catch (Exception)
            {

                throw;
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
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
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
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
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
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
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