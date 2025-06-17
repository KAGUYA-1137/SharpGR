namespace SharpGR_WPF
{
    /// <summary>
    /// 楽曲情報のデータ構造。
    /// </summary>
    public class SongAPI
    {
        public SERVERINFO SERVERINFO { get; set; }

        public SONGINFO SONGINFO { get; set; }

        public SONGTIMES SONGTIMES { get; set; }

        public SONGDATA SONGDATA { get; set; }

        public MISC MISC { get; set; }
    }

    public class SERVERINFO
    {
        public int LISTENERS { get; set; }
    }

#nullable enable

    public class SONGINFO
    {
        /// <summary>
        /// タイトル名
        /// </summary>
        public string? TITLE { get; set; }

        /// <summary>
        /// アーティスト名
        /// </summary>
        public string? ARTIST { get; set; }

        /// <summary>
        /// アルバム名
        /// </summary>
        public string? ALBUM { get; set; }

        /// <summary>
        /// リリース年
        /// </summary>
        public string? YEAR { get; set; }

        /// <summary>
        /// サークル名
        /// </summary>
        public string? CIRCLE { get; set; }
    }

    public class SONGTIMES
    {
        public int DURATION { get; set; }
        public int PLAYED { get; set; }
        public int REMAINING { get; set; }
        public int SONGSTART { get; set; }
        public int SONGEND { get; set; }
    }

    public class SONGDATA
    {
        public int? SONGID { get; set; }
        public int? ALBUMID { get; set; }
        public string? RATING { get; set; }
        public int? TIMESRATED { get; set; }
    }

    public class MISC
    {
        public string? CIRCLELINK { get; set; }

        /// <summary>
        /// アルバムアートのファイル名
        /// </summary>
        public string? ALBUMART { get; set; }

        public string? CIRCLEART { get; set; }
        public string? OFFSET { get; set; }
        public int? OFFSETTIME { get; set; }
    }

#nullable disable
}
