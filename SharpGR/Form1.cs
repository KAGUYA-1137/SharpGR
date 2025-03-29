using System.Reflection;
using NAudio.Wave;
using Newtonsoft.Json;
using SharpGR.FileIO;

namespace SharpGR
{
    /// <summary>
    /// Form1�̃N���X
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
        /// ���C����ʃ��[�h��
        /// </summary>
        private async void Form1_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Text = $"{assembly.GetName().Name} {assembly.GetName().Version}";
            await ReadSettings();
        }

        /// <summary>
        /// �ݒ�ǂݍ���
        /// </summary>
        public async Task ReadSettings()
        {
            // �ݒ�t�@�C�������݂��Ȃ��ꍇ
            if (!File.Exists(Constants.FORM_MAIN_SETTING_FILE_NAME))
            {
                // �ݒ�t�@�C�����쐬���ăf�t�H���g�̐ݒ�l����������
                if (!JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo))
                {
                    ErrorMessage = "�f�t�H���g�̐ݒ�l��ݒ�t�@�C���֏������߂܂���ł���";
                    MessageBox.Show(ErrorMessage, "�G���[���������܂���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    toolStripStatusLabel1.ForeColor = Color.Red;
                    toolStripStatusLabel1.Text = ErrorMessage;
                }
            }

            // �ݒ�t�@�C����ǂݍ��߂���
            if (JsonUtility.ReadJson(Constants.FORM_MAIN_SETTING_FILE_NAME) != null)
            {
                settingInfo = JsonUtility.ReadJson(Constants.FORM_MAIN_SETTING_FILE_NAME);

                // ���ʂ𔽉f����
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

            // �ݒ�t�@�C����ǂݍ��߂Ȃ�������
            else
            {
                ErrorMessage = "�ݒ��ǂݍ��߂܂���ł����A�f�t�H���g�̐ݒ�l���g�p���܂��B";
                MessageBox.Show(ErrorMessage, "�G���[���������܂���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;

                // ���ʂ𔽉f����
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
        /// �Đ�/��~�{�^��������
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
        /// ���z�����W�I���Đ�
        /// </summary>
        private async Task StartRadioAsync()
        {
            try
            {
                waveOut.Init(reader);
                await getSongFromAPIAsync();
                timer1.Start();
                waveOut.Play();
                buttonPlay.Text = "��~";
            }

            catch (Exception ex)
            {
                ErrorMessage = "�Đ����ɃG���[���������܂���";
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

                // �A���o���A�[�g����̂Ƃ�(�C���^�[�~�b�V�����Đ����Ƃ�)
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
        /// ���z�����W�I���~
        /// </summary>
        private void StopRadio()
        {
            try
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    timer1.Stop();
                    buttonPlay.Text = "�Đ�";
                }
            }

            catch (Exception ex)
            {
                ErrorMessage = "��~���ɃG���[���������܂���";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
            }
        }

        private void ������@ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHelp formHelp = new FormHelp();
            formHelp.Show();
        }

        private void sharpGR�ɂ���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.Show();
        }

        /// <summary>
        /// ���ʕύX��(�g���b�N�o�[)
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
                ErrorMessage = "���ʂ̕ύX���ɃG���[���������܂���";
                MessageBox.Show(ErrorMessage, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                toolStripStatusLabel1.ForeColor = Color.Red;
                toolStripStatusLabel1.Text = ErrorMessage;
            }
        }

        /// <summary>
        /// ���ʕύX��(�X�s���{�b�N�X)
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
                ErrorMessage = "���ʂ̕ύX���ɃG���[���������܂���";
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
                case "�Đ�":
                    settingInfo.PlaybackState = PlaybackState.Stopped;
                    settingInfo.Volume = Convert.ToInt32(numericUpDownVol.Text);
                    break;
                case "��~":
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
                MessageBox.Show("�ݒ�̏������ݒ��ɃG���[���������܂���", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}