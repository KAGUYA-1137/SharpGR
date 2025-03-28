﻿namespace SharpGR
{
    public class SongAPI
    {
        public SONGINFO SONGINFO { get; set; }
        public SONGTIMES SONGTIMES { get; set; }
        public SONGDATA SONGDATA { get; set; }
        public MISC MISC { get; set; }
    }

    public class SONGINFO
    {
        public string TITLE { get; set; }
        public string ARTIST { get; set; }
        public string ALBUM { get; set; }
        public string YEAR { get; set; }
        public string CIRCLE { get; set; }
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
        public int SONGID { get; set; }
        public int ALBUMID { get; set; }
        public string RATING { get; set; }
        public int TIMESRATED { get; set; }
    }

    public class MISC
    {
        public string CIRCLELINK { get; set; }
        public string ALBUMART { get; set; }
        public string CIRCLEART { get; set; }
        public string OFFSET { get; set; }
        public int OFFSETTIME { get; set; }
    }

}
