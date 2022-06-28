namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// header information
    /// </summary>
    public class Info
    {
        /// <summary>
        /// self-explanatory
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// self-explanatory
        /// </summary>
        public string? subtitle { get; set; }

        /// <summary>
        /// self-explanatory
        /// </summary>
        public string? artist { get; set; }

        /// <summary>
        /// ["key:value"]
        /// </summary>
        public string[]? subartists { get; set; }

        /// <summary>
        /// self-explanatory
        /// </summary>
        public string? genre { get; set; }

        /// <summary>
        /// layout hints, e.g. "beat-7k", "popn-5k", "generic-nkeys"
        /// </summary>
        public string mode_hint { get; set; } = "beat-7k";

        /// <summary>
        /// e.g. "HYPER", "FOUR DIMENSIONS"
        /// </summary>
        public string? chart_name { get; set; }

        /// <summary>
        /// self-explanatory
        /// </summary>
        public ulong level { get; set; }

        /// <summary>
        /// self-explanatory
        /// </summary>
        public double init_bpm { get; set; }

        /// <summary>
        /// relative judge width
        /// </summary>
        public double judge_rank { get; set; } = 100;

        /// <summary>
        /// relative lifebar gain
        /// </summary>
        public double total { get; set; } = 100;

        /// <summary>
        /// background image filename
        /// </summary>
        public string? back_image { get; set; }

        /// <summary>
        /// eyecatch image filename
        /// </summary>
        public string? eyecatch_image { get; set; }

        /// <summary>
        /// banner image filename
        /// </summary>
        public string? banner_image { get; set; }

        /// <summary>
        /// title image filename
        /// </summary>
        public string? title_image { get; set; }

        /// <summary>
        /// preview music filename
        /// </summary>
        public string? preview_music { get; set; }

        /// <summary>
        /// pulses per quarter note
        /// </summary>
        public int resolution { get; set; } = 240;
    }
}
