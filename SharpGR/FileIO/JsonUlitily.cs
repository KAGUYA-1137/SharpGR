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
        /// JSONからデータを読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="jsonStr">楽曲情報</param>
        /// <returns><see cref="SettingInfo"/> または <see cref="SongAPI"/></returns>
        public static T ReadJson<T>(string? fileName, string? jsonStr)
        {
            Type type = typeof(T);

            try
            {
                if (type == typeof(SettingInfo))
                {
                    string json = File.ReadAllText(fileName);

                    SettingInfo settingInfo = new SettingInfo();
                    settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                    // 設定ファイルから設定を読み込めたら読み込んだ情報を返す
                    return (T)(object)settingInfo;
                }

                if (type == typeof(SongAPI))
                {
                    SongAPI songAPI = new SongAPI();
                    songAPI = JsonConvert.DeserializeObject<SongAPI>(jsonStr);

                    return (T)(object)songAPI;
                }

                else
                {
                    throw new NotSupportedException($"{type} はサポートされていません。");
                }
            }

            catch
            {
                // 読み込めなかったらnullを返す
                return (T)(object)null;
            }
        }
    }
}
