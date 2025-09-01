using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using NAudio.Wave;
using Newtonsoft.Json;
using SharpGR_WPF.ViewModels;
using MessageBox = System.Windows.MessageBox;

namespace SharpGR_WPF.IO
{
    /// <summary>
    /// JSONへの書き込みと読み込みを行うクラス。
    /// </summary>
    public class JsonUtility
    {
        private MainWindowViewModel mainWindowViewModel;

        public JsonUtility(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;
        }

        /// <summary>
        /// 指定されたJSONファイルへ設定値を書き込みます。
        /// </summary>
        /// <param name="filePath">設定値を書き込むファイル名</param>
        /// <param name="settingInfo">書き込む設定値</param>
        /// <returns><see cref="bool"/>。設定を書き込めたらtrue、それ以外はfalse</returns>
        public bool WriteJson(string filePath, SettingInfo settingInfo)
        {
            try
            {
                string json = JsonConvert.SerializeObject(settingInfo, Formatting.Indented);
                File.WriteAllText(filePath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 指定されたJSONファイルから設定値を読み込み、メイン画面に反映します。
        /// </summary>
        /// <param name="filePath">読み込むファイル</param>
        /// <returns><see cref="SettingInfo"/></returns>
        public void ReadSettingFromJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                SettingInfo settingInfo = new SettingInfo();
                settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                // メイン画面の音量テキストボックスに設定ファイルの音量値を反映
                mainWindowViewModel.Volume = settingInfo.Volume;
                mainWindowViewModel.waveOutEvent.Volume = (float)(settingInfo.Volume / 100.0);

                if (settingInfo.PlaybackState == PlaybackState.Playing)
                {
                    // 設定ファイルに記載されている再生状態が「再生」か
                    mainWindowViewModel.PlayButtonText = "一時停止";
                    mainWindowViewModel.StartRadio();
                }
                else
                {
                    // 「一時停止」なら
                    mainWindowViewModel.PlayButtonText = "再生";
                    mainWindowViewModel.StopRadio();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("JSONの読み込みに失敗しました。", ex.Message, MessageBoxButton.OK, MessageBoxImage.Hand);
                return;
            }
        }

        /// <summary>
        /// 幻想郷ラジオからのレスポンスボディをパースし、メイン画面に反映
        /// </summary>
        /// <param name="response">幻想郷ラジオのAPIからのレスポンスボディ</param>
        /// <param name="mainWindowViewModel">メイン画面のビューモデル</param>
        public void ParseAndSetDataFromResponse(string response, MainWindowViewModel mainWindowViewModel)
        {
            //await Task.Run(() =>
            //{
            try
            {
                //var RadioAPI = new RadioAPI();
                mainWindowViewModel.RadioAPI = JsonConvert.DeserializeObject<RadioAPI>(response);
                mainWindowViewModel.NameLabelText = mainWindowViewModel.RadioAPI.SONGINFO.TITLE;
                mainWindowViewModel.ArtistLabelText = mainWindowViewModel.RadioAPI.SONGINFO.ARTIST;
                mainWindowViewModel.AlbumNameLabelText = mainWindowViewModel.RadioAPI.SONGINFO.ALBUM;
                mainWindowViewModel.RadioAPI.SONGTIMES.REMAINING = mainWindowViewModel.RadioAPI.SONGTIMES.REMAINING;
                mainWindowViewModel.RadioAPI.SONGDATA.ALBUMID = mainWindowViewModel.RadioAPI.SONGDATA.ALBUMID;
                mainWindowViewModel.TimeLabelText = $"{TimeSpan.FromSeconds(mainWindowViewModel.RadioAPI.SONGTIMES.PLAYED).ToString(@"m\:ss")} / {TimeSpan.FromSeconds(mainWindowViewModel.RadioAPI.SONGTIMES.DURATION).ToString(@"m\:ss")}";

                string albumArtPath = mainWindowViewModel.RadioAPI.MISC.ALBUMART;
                if (!string.IsNullOrWhiteSpace(albumArtPath))
                {
                    string fullAlbumArtPath = $"{Constants.AlbumArtPrefixURL}{albumArtPath}";

                    try
                    {
                        mainWindowViewModel.AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(fullAlbumArtPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "アルバムアートの読み込みに失敗しました。", MessageBoxButton.OK, MessageBoxImage.Hand);
                        mainWindowViewModel.AlbumArtImageSource = null;
                    }
                }
                else
                {
                    mainWindowViewModel.AlbumArtImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(Constants.PlaceholderAlbumArtURL);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "レスポンスボディのパースに失敗しました。", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            //});
        }

        ///// <summary>
        ///// レスポンスからタイトルを取得（曲が変わったことを検知するために必要）
        ///// </summary>
        ///// <param name="response"></param>
        ///// <returns></returns>
        //public static string GetTitleFromResponse(string response)
        //{
        //    try
        //    {
        //        JObject json = JObject.Parse(response);
        //        JObject songInfo = (JObject)json["SONGINFO"];
        //        return songInfo?["TITLE"]?.ToString() ?? Constants.Blank;
        //    }
        //    catch
        //    {
        //        return Constants.Blank;
        //    }
        //}
    }
}