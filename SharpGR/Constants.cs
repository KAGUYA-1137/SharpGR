namespace SharpGR
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 設定ファイルのディレクトリ
        /// </summary>
        public const string SETTINGS_FOOTER = "Setting\\";

        /// <summary>
        /// メイン画面の設定ファイル名
        /// </summary>
        public const string FORM_MAIN_SETTING_FILE_NAME = $"{SETTINGS_FOOTER}Form1.json";

        /// <summary>
        /// 幻想郷ラジオのストリームエンドポイント
        /// </summary>
        public const string STREAM_ENDPOINT = "https://stream.gensokyoradio.net/3";

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報の取得先
        /// </summary>
        public const string MUSIC_INFO_JSON = "https://gensokyoradio.net/api/station/playing/";

        /// <summary>
        /// 幻想郷ラジオのアルバムアート取得先の絶対パス
        /// </summary>
        public const string ALBUM_ART_PREFIX = "https://gensokyoradio.net/images/albums/500/";
    }
}
