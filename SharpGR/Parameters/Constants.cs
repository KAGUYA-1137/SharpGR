namespace SharpGR.Property
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 設定ファイルのディレクトリ
        /// </summary>
        public const string SETTING_FOOTER = @"Setting\";

        /// <summary>
        /// 設定が記載されたファイル名
        /// </summary>
        public const string SETTING_FILE_NAME = "Setting.json";

        /// <summary>
        /// 幻想郷ラジオのストリームエンドポイント
        /// </summary>
        public const string STREAM_ENDPOINT = "https://stream.gensokyoradio.net/3";

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報の取得先
        /// </summary>
        public const string PLAYING_API_URL = "https://gensokyoradio.net/api/station/playing/";

        /// <summary>
        /// 幻想郷ラジオのアルバムアート取得先の絶対パス
        /// </summary>
        public const string ALBUM_ART_PREFIX = "https://cdn.gensokyoradio.net/images/albums/500/";

        /// <summary>
        /// 幻想郷ラジオのデフォルトのアルバムアートのパス
        /// </summary>
        public const string LOGO_PLACE_HOLDER_URL = "https://cdn.gensokyoradio.net/images/assets/gr-logo-placeholder.png";

        /// <summary>
        /// 幻想郷ラジオのアルバム情報のURL
        /// </summary>
        public const string ALBUM_INFO_PREFIX = "https://gensokyoradio.net/music/album/";

        /// <summary>
        /// 空文字
        /// </summary>
        public const string BLANK = "----";
    }
}