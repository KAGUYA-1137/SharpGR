using System.Reflection;
using NAudio.Wave;
using SharpGR.FileIO;

namespace SharpGR
{
    public partial class Form1 : Form
    {
        private WaveOutEvent waveOut = new WaveOutEvent();
        private readonly MediaFoundationReader reader = new MediaFoundationReader("https://stream.gensokyoradio.net/3");
        private SettingInfo settingInfo;

        ///// <summary>
        ///// 閉じるボタンを非表示
        ///// </summary>
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        const int CS_CLOSE = 0x200;

        //        CreateParams createParams = base.CreateParams;
        //        createParams.ClassStyle |= CS_CLOSE;

        //        return createParams;
        //    }
        //}

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Text = $"{assembly.GetName().Name} {assembly.GetName().Version}";
            ReadSettings();
        }

        public void ReadSettings()
        {
            // SettingInfoクラスをNew
            settingInfo = new SettingInfo();

            // 設定ファイルが存在しない場合
            if (!File.Exists(Constants.FORM_MAIN_SETTING_FILE_NAME))
            {
                // デフォルトの設定値をメイン画面の設定ファイルへ書き込み(WriteJsonでF12キーを押すと実装元に飛べます)
                JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo);
            }

            try
            {
                // メイン画面の設定ファイルを読み込み
                settingInfo = JsonUtility.ReadJson(Constants.FORM_MAIN_SETTING_FILE_NAME);
                // 音量を設定する
                trackBarVol.Value = Convert.ToInt32(settingInfo.Volume);
                numericUpDownVol.Text = settingInfo.Volume.ToString();
                if (settingInfo.PlaybackState == PlaybackState.Playing && waveOut.PlaybackState != PlaybackState.Playing)
                {
                    StartRadio();
                }
                else
                {
                    StopRadio();
                }
            }
            catch
            {
                JsonUtility.WriteJson(Constants.FORM_MAIN_SETTING_FILE_NAME, settingInfo);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                StopRadio();
            }
            else
            {
                StartRadio();
            }
        }

        private void StartRadio()
        {
            try
            {
                waveOut.Init(reader);
                waveOut.Play();
                buttonPlay.Text = "停止";
            }
            catch (Exception ex)
            {
                MessageBox.Show("再生中にエラーが発生しました: " + ex.Message);
            }
        }

        private void StopRadio()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                buttonPlay.Text = "再生";
            }
        }

        private void trackBarVol_Scroll(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Volume = trackBarVol.Value / 100f;
                numericUpDownVol.Text = $"{trackBarVol.Value}%";
            }
        }

        private void 操作方法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHelp formHelp = new FormHelp();
            formHelp.Show();
        }

        private void trackBarVol_ValueChanged(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Volume = trackBarVol.Value / 100f;
                numericUpDownVol.Text = $"{trackBarVol.Value}%";
            }
        }

        private void sharpGRについてToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            formAbout.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SettingInfo settingInfo = new SettingInfo();
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