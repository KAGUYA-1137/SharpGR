using Newtonsoft.Json;

namespace SharpGR.FileIO
{
    /// <summary>
    /// 設定ファイルへの書き込みと読み込みを行うクラス
    /// </summary>
    public class JsonUtility
    {
        /// <summary>
        /// 設定ファイルへ書き込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="data">書き込むデータ</param>
        public static void WriteJson(string fileName, SettingInfo data)
        {
            SettingInfo settingInfo = new SettingInfo();
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }

        /// <summary>
        /// 設定ファイルから設定読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns></returns>
        public static SettingInfo ReadJson(string fileName)
        {
            string json = string.Empty;
            SettingInfo settingInfo = new SettingInfo();
            try
            {
                json = File.ReadAllText(fileName);
                settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);
            }
            catch
            {
                throw;
            }
            finally
            {
                json = string.Empty;
            }
            return settingInfo;
        }
    }
}
