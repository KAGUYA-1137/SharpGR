namespace SharpGR_WPF
{
    public class Constants
    {
        /// <summary>
        /// 設定ファイルのディレクトリ
        /// </summary>
        public const string SETTING_FOOTER = "Setting\\";

        /// <summary>
        /// メイン画面の設定が記載されたファイル名
        /// </summary>
        public const string FORM_MAIN_SETTING_FILE_NAME = $"{SETTING_FOOTER}Form1.json";

        /// <summary>
        /// 幻想郷ラジオのストリームエンドポイント
        /// </summary>
        public const string STREAM_ENDPOINT = "https://stream.gensokyoradio.net/3";

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報の取得先
        /// </summary>
        public const string SONG_INFO_API = "https://gensokyoradio.net/api/station/playing/";

        /// <summary>
        /// 幻想郷ラジオのアルバムアート取得先の絶対パス
        /// </summary>
        public const string ALBUM_ART_PREFIX = "https://gensokyoradio.net/images/albums/500/";

        /// <summary>
        /// 幻想郷ラジオのデフォルトのアルバムアートのパス
        /// </summary>
        public const string ALBUM_ART_PLACEHOLDER = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";

        /// <summary>
        /// 幻想郷ラジオのアルバム情報のURL
        /// </summary>
        public const string ALBUM_INFO_LINK = "https://gensokyoradio.net/music/album/";

        public const string BLANK = "----";
    }
}
