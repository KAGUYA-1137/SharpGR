using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpGR_WPF.ViewModels;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace SharpGR_WPF.IO
{
    /// <summary>
    /// JSONへの書き込みと読み込みを行うクラス。
    /// </summary>
    public class JsonUtility
    {
        /// <summary>
        /// 指定されたJSONファイルへ設定値を書き込みます。
        /// </summary>
        /// <param name="filePath">ファイル名</param>
        /// <param name="settingInfo">書き込むデータ</param>
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
        /// 指定されたJSONファイルから設定値を読み込みます。
        /// </summary>
        /// <param name="filePath">読み込むファイル</param>
        /// <returns><see cref="SettingInfo"/></returns>
        public SettingInfo ReadSettingJson(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);

                SettingInfo settingInfo = new SettingInfo();
                settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                // 設定ファイルから設定を読み込めたら設定値を返す
                return settingInfo;
            }

            catch (Exception ex)
            {
                _ = MessageBox.Show("JSONの読み込みに失敗しました。", ex.Message, MessageBoxButton.OK, MessageBoxImage.Hand);

                // 読み込めなかったら、nullを返す
                return null;
            }
        }

        /// <summary>
        /// 幻想郷ラジオからのレスポンスボディをパースし、メイン画面に反映
        /// </summary>
        /// <param name="response">幻想郷ラジオのAPIからのレスポンスボディ</param>
        /// <param name="mainWindowViewModel">メイン画面のビューモデル</param>
        public void ParseAndSetData(string response, MainWindowViewModel mainWindowViewModel)
        {
            try
            {
                JObject json = JObject.Parse(response);

                JObject songInfo = (JObject)json["SONGINFO"];
                if (songInfo == null)
                {
                    mainWindowViewModel.NameLabelText = songInfo["TITLE"]?.ToString() ?? Constants.Blank;
                    mainWindowViewModel.ArtistLabelText = songInfo["ARTIST"].ToString() ?? Constants.Blank;
                    mainWindowViewModel.AlbumNameLabelText = songInfo["ALBUM"].ToString();
                }

                string albumArtPath = json["MISC"]?["ALBUMART"]?.ToString();
                if (!string.IsNullOrWhiteSpace(albumArtPath))
                {
                    string fullUrl = $"{Constants.AlbumArtPrefixURL}{albumArtPath}";

                    Task.Run(async () =>
                    {
                        try
                        {
                            using HttpClient httpClient = new HttpClient();
                            byte[] imageData = await httpClient.GetByteArrayAsync(fullUrl);
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.StreamSource = new MemoryStream(imageData);
                            bitmapImage.EndInit();
                            bitmapImage.Freeze();

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                mainWindowViewModel.AlbumArtImageSource = bitmapImage;
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "アルバムアートの読み込みに失敗しました。", MessageBoxButton.OK, MessageBoxImage.Hand);
                            mainWindowViewModel.AlbumArtImageSource = null;
                        }
                    });
                }

                else
                {
                    mainWindowViewModel.AlbumArtImageSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}", "レスポンスボディのパースに失敗しました。", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        /// <summary>
        /// レスポンスからタイトルを取得（曲が変わったことを検知するために必要）
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetTitleFromResponse(string response)
        {
            try
            {
                JObject json = JObject.Parse(response);
                JObject songInfo = (JObject)json["SONGINFO"];
                return songInfo?["TITLE"]?.ToString() ?? Constants.Blank;
            }
            catch
            {
                return Constants.Blank;
            }
        }
    }
}