namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// sound channel
    /// </summary>
    public class Sound_Channels
    {
        /// <summary>
        /// sound file name
        /// </summary>
        public string name { get; set; } = null!;

        /// <summary>
        /// notes using this sound
        /// </summary>
        public Note[] notes { get; set; } = null!;
    }
}
