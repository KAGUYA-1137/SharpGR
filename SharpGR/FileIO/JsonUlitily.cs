using Newtonsoft.Json;

namespace SharpGR.FileIO
{
    /// <summary>
    /// 設定ファイルへの書き込みと読み込みを行うクラス
    /// </summary>
    public class JsonUtility
    {
        /// <summary>
        /// 設定ファイルへ設定を書き込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="data">書き込むデータ</param>
        /// <returns><see cref="bool"/>。設定を書き込めたらtrue、それ以外はfalse</returns>
        public static bool WriteJson(string fileName, SettingInfo data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fileName, json);
                return true;
            }

            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 設定ファイルから設定を読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns><see cref="SettingInfo"/></returns>
        public static SettingInfo? ReadJson(string fileName)
        {
            SettingInfo settingInfo = new SettingInfo();

            try
            {
                string json = File.ReadAllText(fileName);
                settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                // 設定ファイルから設定を読み込めたら読み込んだ情報を返す
                return settingInfo;
            }

            catch
            {
                // 読み込めなかったらnullを返す
                return null;
            }
        }
    }
}
