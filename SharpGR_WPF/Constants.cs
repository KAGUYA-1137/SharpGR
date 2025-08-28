namespace SharpGR_WPF
{
    public static class Constants
    {
        /// <summary>
        /// 設定ファイルのディレクトリ
        /// </summary>
        public const string SettingFooter = "Setting\\";

        /// <summary>
        /// メイン画面の設定が記載されたファイル名
        /// </summary>
        public const string MainWindowSettingFileName = $"{SettingFooter}Setting.json";

        /// <summary>
        /// 幻想郷ラジオのストリームエンドポイント
        /// </summary>
        public const string StreamEndpoint = "https://stream.gensokyoradio.net/3";

        /// <summary>
        /// 幻想郷ラジオで再生中の楽曲情報の取得先
        /// </summary>
        public const string RadioAPIURL = "https://gensokyoradio.net/api/station/playing/";

        /// <summary>
        /// 幻想郷ラジオのアルバムアート取得先の絶対パス
        /// </summary>
        public const string AlbumArtPrefixURL = "https://gensokyoradio.net/images/albums/500/";

        /// <summary>
        /// 幻想郷ラジオのデフォルトのアルバムアートのパス
        /// </summary>
        public const string PlaceholderAlbumArtURL = "https://gensokyoradio.net/images/assets/gr-logo-placeholder.png";

        /// <summary>
        /// 幻想郷ラジオのアルバム情報のURL
        /// </summary>
        public const string AlbumInfoURL = "https://gensokyoradio.net/music/album/";

        /// <summary>
        /// 空文字（曲情報用）
        /// </summary>
        public const string Blank = "----";

        /// <summary>
        /// 空文字（タイマー用）
        /// </summary>
        public const string BlankTimer = "--:--/--:--";
    }
}
