

namespace BeMSic.Bmson
{
    // top-level object
    public class Bmson
    {
        // bmson version
        public string version { get; set; }

        // information, e.g. title, artist, …
        public Info info { get; set; }

        // location of bar-lines in pulses
        public Line[]? lines { get; set; }

        // bpm changes
        public BpmEvents[]? bpm_events { get; set; }

        // stop events
        public BpmEvents[]? stop_events { get; set; }

        // note data
        public Sound_Channels[] sound_channels { get; set; }

        // bga data
        public BGA bga { get; set; }
    }

    // header information
    public class Info
    {
        // self-explanatory
        public string title { get; set; }

        // self-explanatory
        public string subtitle { get; set; }

        // self-explanatory
        public string artist { get; set; }

        // ["key:value"]
        public string[]? subartists { get; set; }

        // self-explanatory
        public string genre { get; set; }

        // layout hints, e.g. "beat-7k", "popn-5k", "generic-nkeys"
        public string mode_hint { get; set; } = "beat-7k";

        // e.g. "HYPER", "FOUR DIMENSIONS"
        public string chart_name { get; set; }

        // self-explanatory
        public ulong level { get; set; }

        // self-explanatory
        public double init_bpm { get; set; }

        // relative judge width
        public double judge_rank { get; set; } = 100;

        // relative lifebar gain
        public double total { get; set; } = 100;

        // background image filename
        public string? back_image { get; set; }

        // eyecatch image filename
        public string? eyecatch_image { get; set; }

        // banner image filename
        public string? banner_image { get; set; }

        // title image filename
        public string title_image { get; set; }

        // preview music filename
        public string? preview_music { get; set; }

        // pulses per quarter note
        public int resolution { get; set; } = 240;
    }

    // bar-line event
    public class Line
    {
        // pulse number
        public ulong y { get; set; }
    }

    // bpm note
    public class BpmEvents
    {
        // pulse number
        public ulong y { get; set; }

        // bpm
        public double bpm { get; set; }
    }

    // stop note
    public class StopEvent
    {
        // pulse number
        public ulong y { get; set; }

        // stop duration (pulses to stop)
        public ulong duration { get; set; }
    }

    // sound channel
    public class Sound_Channels
    {
        // sound file name
        public string name { get; set; }

        // notes using this sound
        public Note[] notes { get; set; }
    }

    // sound note
    public class Note
    {
        // lane
        public int x { get; set; }

        // pulse number
        public ulong y { get; set; }

        // length (0: normal note; greater than zero (length in pulses): long note)
        public ulong l { get; set; }

        // continuation flag
        public bool c { get; set; }
    }

    // bga
    public class BGA
    {
        // picture id and filename
        public BGAHeader[] bga_header { get; set; }

        // picture sequence
        public BGAEvent[] bga_events { get; set; }

        // picture sequence overlays bga_notes
        public BGAEvent[] layer_events { get; set; }

        // picture sequence when missed
        public BGAEvent[] poor_events { get; set; }
    }

    // picture file
    public class BGAHeader
    {
        // self-explanatory
        public ulong id { get; set; }

        // picture file name
        public string name { get; set; } = "";
    }

    // bga note
    public class BGAEvent
    {
        // pulse number
        public ulong y { get; set; }

        // corresponds to BGAHeader.id
        public ulong id { get; set; }
    }
}
