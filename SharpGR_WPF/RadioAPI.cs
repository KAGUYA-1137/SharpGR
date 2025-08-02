namespace SharpGR_WPF
{
#nullable enable
    /// <summary>
    /// 幻想郷ラジオのAPI
    /// </summary>
    public class RadioAPI
    {
        /// <summary>
        /// サーバー情報
        /// </summary>
        public SERVERINFO? SERVERINFO { get; set; }

        /// <summary>
        /// 楽曲情報
        /// </summary>
        public SONGINFO? SONGINFO { get; set; }

        /// <summary>
        /// 楽曲の時間情報
        /// </summary>
        public SONGTIMES? SONGTIMES { get; set; }

        /// <summary>
        /// 楽曲データ
        /// </summary>
        public SONGDATA? SONGDATA { get; set; }

        /// <summary>
        /// その他
        /// </summary>
        public MISC? MISC { get; set; }
    }

    /// <summary>
    /// サーバー情報
    /// </summary>
    public class SERVERINFO
    {
        /// <summary>
        /// 最終更新日
        /// </summary>
        public int LASTUPDATE { get; set; }

        /// <summary>
        /// サーバー数
        /// </summary>
        public int SERVERS { get; set; }

        /// <summary>
        /// ステータス
        /// </summary>
        public string? STATUS { get; set; }

        /// <summary>
        /// 現在の鑑賞人数
        /// </summary>
        public int LISTENERS { get; set; }

        /// <summary>
        /// ストリーム情報
        /// </summary>
        public STREAMS? STREAMS { get; set; }

        /// <summary>
        /// モード
        /// </summary>
        public string? MODE { get; set; }
    }

    /// <summary>
    /// ストリーム情報
    /// </summary>
    public class STREAMS
    {
        /// <summary>
        /// 1つ目のストリーム情報
        /// </summary>
        public _1? _1 { get; set; }

        /// <summary>
        /// 2つ目のストリーム情報
        /// </summary>
        public _2? _2 { get; set; }

        /// <summary>
        /// 3つ目のストリーム情報
        /// </summary>
        public _3? _3 { get; set; }

        /// <summary>
        /// 4つ目のストリーム情報
        /// </summary>
        public _4? _4 { get; set; }

        /// <summary>
        /// 5つ目のストリーム情報
        /// </summary>
        public _5? _5 { get; set; }
    }

    /// <summary>
    /// 1つ目のストリーム情報
    /// </summary>
    public class _1
    {
        public int BITRATE { get; set; }
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 2つ目のストリーム情報
    /// </summary>
    public class _2
    {
        public int BITRATE { get; set; }
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 3つ目のストリーム情報
    /// </summary>
    public class _3
    {
        public int BITRATE { get; set; }
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 4つ目のストリーム情報
    /// </summary>
    public class _4
    {
        public int BITRATE { get; set; }
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 5つ目のストリーム情報
    /// </summary>
    public class _5
    {
        public string? BITRATE { get; set; }
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 楽曲情報
    /// </summary>
    public class SONGINFO
    {
        public string? TITLE { get; set; }
        public string? ARTIST { get; set; }
        public string? ALBUM { get; set; }
        public string? YEAR { get; set; }
        public string? CIRCLE { get; set; }
    }

    /// <summary>
    /// 楽曲の時間情報
    /// </summary>
    public class SONGTIMES
    {
        public int DURATION { get; set; }
        public int PLAYED { get; set; }
        public int REMAINING { get; set; }
        public int SONGSTART { get; set; }
        public int SONGEND { get; set; }
    }

    /// <summary>
    /// 楽曲データ
    /// </summary>
    public class SONGDATA
    {
        public int SONGID { get; set; }
        public int? ALBUMID { get; set; }
        public string? RATING { get; set; }
        public int TIMESRATED { get; set; }
    }

    /// <summary>
    /// その他
    /// </summary>
    public class MISC
    {
        public string? CIRCLELINK { get; set; }
        public string? ALBUMART { get; set; }
        public string? CIRCLEART { get; set; }
        public string? OFFSET { get; set; }
        public int OFFSETTIME { get; set; }
    }
#nullable disable
}
