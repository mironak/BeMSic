namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// top-level object
    /// </summary>
    public class BmsonFormat
    {
        /// <summary>
        /// bmson version
        /// </summary>
        public string? version { get; set; }

        /// <summary>
        /// information, e.g. title, artist, …
        /// </summary>
        public Info info { get; set; } = null!;

        /// <summary>
        /// location of bar-lines in pulses
        /// </summary>
        public Line[] lines { get; set; } = null!;

        /// <summary>
        /// bpm changes
        /// </summary>
        public BpmEvents[]? bpm_events { get; set; }

        /// <summary>
        /// stop events
        /// </summary>
        public BpmEvents[]? stop_events { get; set; }

        /// <summary>
        /// note data
        /// </summary>
        public Sound_Channels[] sound_channels { get; set; } = null!;

        /// <summary>
        /// bga data
        /// </summary>
        public BGA? bga { get; set; }
    }
}
