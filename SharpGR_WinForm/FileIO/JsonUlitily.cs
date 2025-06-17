using Newtonsoft.Json;

namespace SharpGR_WinForm.FileIO
{
    /// <summary>
    /// JSONへの書き込みと読み込みを行うクラス。
    /// </summary>
    public class JsonUtility
    {
        /// <summary>
        /// 設定ファイルへ設定値を書き込みます。
        /// </summary>
        /// <param name="path">ファイル名</param>
        /// <param name="settingInfo">書き込むデータ</param>
        /// <returns><see cref="bool"/>。設定を書き込めたらtrue、それ以外はfalse</returns>
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
        /// JSONファイルまたはJSON文字列を読み込みを行うクラス。
        /// </summary>
        /// <param name="path">読み込むファイル</param>
        /// <param name="jsonStr">JSON文字列</param>
        /// <returns><see cref="SettingInfo"/> または <see cref="SongAPI"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static T? ReadJson<T>(string? path, string? jsonStr)
        {
            Type type = typeof(T);

            try
            {
                if (type == typeof(SettingInfo))
                {
                    // 設定ファイルを読み込むならファイルパスがないと処理出来ないのでここで検証
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        string json = File.ReadAllText(path);

                        SettingInfo? settingInfo = new SettingInfo();
                        settingInfo = JsonConvert.DeserializeObject<SettingInfo>(json);

                        // 設定ファイルから設定を読み込めたら設定値を返す
                        return (T?)(object?)settingInfo;
                    }

                    // 設定ファイルのパスが空白またはnullの場合
                    else
                    {
                        throw new ArgumentNullException(path, "設定ファイルのパスが指定されていません");
                    }
                }

                if (type == typeof(SongAPI))
                {
                    // 楽曲情報を読み込むならレスポンスボディがないと処理出来ないのでここで検証
                    if (!string.IsNullOrWhiteSpace(jsonStr))
                    {
                        SongAPI? songAPI = new SongAPI();
                        songAPI = JsonConvert.DeserializeObject<SongAPI>(jsonStr);

                        // レスポンスボディから楽曲情報を読み込めたら楽曲情報を返す
                        return (T?)(object?)songAPI;
                    }

                    // レスポンスボディが空白またはnullの場合
                    else
                    {
                        throw new ArgumentNullException(jsonStr, "楽曲情報が提供されていません");
                    }
                }

                // ここに来るということは変なものを渡したということ
                else
                {
                    throw new NotSupportedException($"{type} には対応していません");
                }
            }

            catch (Exception ex)
            {
                _ = MessageBox.Show("JSONの読み込みに失敗しました。", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 読み込めなかったら、nullを返す
                return (T?)(object?)null;
            }
        }
    }
}