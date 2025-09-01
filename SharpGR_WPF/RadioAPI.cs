namespace SharpGR_WPF
{
#nullable enable
    /// <summary>
    /// 幻想郷ラジオのAPIからのレスポンスボディの構造
    /// </summary>
    public class RadioAPI
    {
        /// <summary>
        /// サーバー情報
        /// </summary>
        public SERVERINFO SERVERINFO { get; set; }

        /// <summary>
        /// 楽曲情報
        /// </summary>
        public SONGINFO SONGINFO { get; set; }

        /// <summary>
        /// 楽曲の時間情報
        /// </summary>
        public SONGTIMES SONGTIMES { get; set; }

        /// <summary>
        /// 楽曲データ
        /// </summary>
        public SONGDATA SONGDATA { get; set; }

        /// <summary>
        /// その他
        /// </summary>
        public MISC MISC { get; set; }
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
        /// 1つ目のサーバーのストリーム情報
        /// </summary>
        public _1? _1 { get; set; }

        /// <summary>
        /// 2つ目のサーバーのストリーム情報
        /// </summary>
        public _2? _2 { get; set; }

        /// <summary>
        /// 3つ目のサーバーのストリーム情報
        /// </summary>
        public _3? _3 { get; set; }

        /// <summary>
        /// 4つ目のサーバーのストリーム情報
        /// </summary>
        public _4? _4 { get; set; }

        /// <summary>
        /// 5つ目のサーバーのストリーム情報
        /// </summary>
        public _5? _5 { get; set; }
    }

    /// <summary>
    /// 1つ目のストリーム情報
    /// </summary>
    public class _1
    {
        /// <summary>
        /// ビットレート
        /// </summary>
        public int BITRATE { get; set; }

        /// <summary>
        /// 聴いている人数
        /// </summary>
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 2つ目のストリーム情報
    /// </summary>
    public class _2
    {
        /// <summary>
        /// ビットレート
        /// </summary>
        public int BITRATE { get; set; }

        /// <summary>
        /// 聴いている人数
        /// </summary>
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 3つ目のストリーム情報
    /// </summary>
    public class _3
    {
        /// <summary>
        /// ビットレート
        /// </summary>
        public int BITRATE { get; set; }

        /// <summary>
        /// 聴いている人数
        /// </summary>
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 4つ目のストリーム情報
    /// </summary>
    public class _4
    {
        /// <summary>
        /// ビットレート
        /// </summary>
        public int? BITRATE { get; set; }

        /// <summary>
        /// 聴いている人数
        /// </summary>
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 5つ目のストリーム情報
    /// </summary>
    public class _5
    {
        /// <summary>
        /// ビットレート
        /// </summary>
        public int? BITRATE { get; set; }

        /// <summary>
        /// 聴いている人数
        /// </summary>
        public int LISTENERS { get; set; }
    }

    /// <summary>
    /// 楽曲情報
    /// </summary>
    public class SONGINFO
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string? TITLE { get; set; }

        /// <summary>
        /// アーティスト名
        /// </summary>
        public string? ARTIST { get; set; }

        /// <summary>
        /// アルバム名
        /// </summary>
        public string ALBUM { get; set; }

        /// <summary>
        /// リリース年
        /// </summary>
        public string? YEAR { get; set; }

        /// <summary>
        /// サークル名
        /// </summary>
        public string? CIRCLE { get; set; }
    }

    /// <summary>
    /// 楽曲の時間情報
    /// </summary>
    public class SONGTIMES
    {
        /// <summary>
        /// 楽曲の総再生時間
        /// </summary>
        public int DURATION { get; set; }

        /// <summary>
        /// 経過時間
        /// </summary>
        public int PLAYED { get; set; }

        /// <summary>
        /// 残りの再生時間
        /// </summary>
        public int REMAINING { get; set; }

        /// <summary>
        /// 楽曲の開始時間
        /// </summary>
        public int SONGSTART { get; set; }

        /// <summary>
        /// 楽曲の終了時間
        /// </summary>
        public int SONGEND { get; set; }
    }

    /// <summary>
    /// 楽曲データ
    /// </summary>
    public class SONGDATA
    {
        /// <summary>
        /// 楽曲ID
        /// </summary>
        public int SONGID { get; set; }

        /// <summary>
        /// アルバムID
        /// </summary>
        public int? ALBUMID { get; set; }

        /// <summary>
        /// 評価
        /// </summary>
        public string? RATING { get; set; }

        /// <summary>
        /// タイムス評価
        /// </summary>
        public int? TIMESRATED { get; set; }
    }

    /// <summary>
    /// その他
    /// </summary>
    public class MISC
    {
        /// <summary>
        /// サークルのリンク
        /// </summary>
        public string? CIRCLELINK { get; set; }

        /// <summary>
        /// アルバムアートのファイル名
        /// </summary>
        public string? ALBUMART { get; set; }

        /// <summary>
        /// サークルのアートワーク
        /// </summary>
        public string? CIRCLEART { get; set; }

        /// <summary>
        /// オフセット
        /// </summary>
        public string? OFFSET { get; set; }

        /// <summary>
        /// オフタイム
        /// </summary>
        public int OFFSETTIME { get; set; }
    }
#nullable disable
}
