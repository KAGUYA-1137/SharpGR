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
        /// <param name="path">ファイル名</param>
        /// <param name="settingInfo">書き込むデータ</param>
        /// <returns>設定を書き込めたらtrue、それ以外はfalse</returns>
        public static bool WriteJson(string path, SettingInfo settingInfo)
        {
            try
            {
                string json = JsonConvert.SerializeObject(settingInfo, Formatting.Indented);
                File.WriteAllText(path, json);
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
        /// <param name="path">読み込むファイル</param>
        /// <param name="value">JSON文字列</param>
        /// <returns><see cref="SettingInfo"/> または <see cref="SongAPI"/></returns>
        public static T? ReadJson<T>(string? path, string? value)
        {
            Type type = typeof(T);

            try
            {
                if (type == typeof(SettingInfo))
                {
                    // 設定ファイルを読み込むならファイル名がないと処理出来ないのでここで検証
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string json = File.ReadAllText(path);

                        SettingInfo? settingInfo = new SettingInfo();
                        settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                        // 設定ファイルから設定を読み込めたら設定値を返す
                        return (T?)(object?)settingInfo;
                    }

                    else
                    {
                        return (T?)(object?)null;
                    }
                }

                if (type == typeof(SongAPI))
                {
                    // 楽曲情報を読み込むならレスポンスボディがないと処理出来ないのでここで検証
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        SongAPI? songAPI = new SongAPI();
                        songAPI = JsonConvert.DeserializeObject<SongAPI>(value);

                        // レスポンスボディから楽曲情報を読み込めたら楽曲情報を返す
                        return (T?)(object?)songAPI;
                    }

                    else
                    {
                        return (T?)(object?)null;
                    }
                }

                // ここに来るということは変なものを渡したということ
                else
                {
                    throw new NotSupportedException($"{type} には対応していません。");
                }
            }

            catch
            {
                // 読み込めなかったら、nullを返す
                return (T?)(object?)null;
            }
        }
    }
}
