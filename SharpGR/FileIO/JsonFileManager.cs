using System.IO;
using Newtonsoft.Json;
using SharpGR.Property;

namespace SharpGR.FileIO
{
    /// <summary>
    /// JSONの読み書きを行うクラスです
    /// </summary>
    public class JsonFileManager
    {
        /// <summary>
        /// 指定されたJSONファイルへ設定値を書き込みます
        /// </summary>
        /// <param name="filePath">設定値を書き込むファイル名</param>
        /// <param name="settingInfo">書き込む設定値</param>
        public void SaveSetting(string filePath, SettingInfo settingInfo)
        {
            try
            {
                // SettingInfoクラスのインスタンスをJSON文字列にシリアライズする
                var jsonStr = JsonConvert.SerializeObject(settingInfo, Formatting.Indented);

                // JSON文字列をファイルに書き込む
                File.WriteAllText(filePath, jsonStr);
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 指定されたJSONファイルから設定値を読み込みます
        /// </summary>
        /// <param name="filePath">読み込むファイル</param>
        /// <returns>読み込んだ設定値を<see cref="SettingInfo"/>として返します
        /// 読み込めなかった場合は <c>null</c> を返します</returns>
        public SettingInfo LoadSetting(string filePath)
        {
            try
            {
                // ファイルからJSON文字列を読み込む
                var json = File.ReadAllText(filePath);

                // JSON文字列をパースして、SettingInfoクラスのインスタンスを生成
                var settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                // 読み込んだ設定値を返す
                return settingInfo;
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 幻想郷ラジオからのレスポンスボディと等価の <see cref="RadioAPI"/> を返します
        /// </summary>
        /// <param name="response">幻想郷ラジオからのレスポンスボディ</param>
        /// <returns>レスポンスボディと等価の<see cref="RadioAPI"/>パースに失敗した場合は <c>null</c> を返します</returns>
        public RadioAPI ParseResponse(string response)
        {
            try
            {
                // レスポンスボディをパースして、RadioAPIクラスのインスタンスを生成して返す
                return JsonConvert.DeserializeObject<RadioAPI>(response);
            }
            // 例外が発生した
            catch
            {
                throw;
            }
        }
    }
}