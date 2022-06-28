namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// sound note
    /// </summary>
    public class Note
    {
        /// <summary>
        /// lane
        /// </summary>
        public int x { get; set; }

        /// <summary>
        /// pulse number
        /// </summary>
        public ulong y { get; set; }

        /// <summary>
        /// length (0: normal note; greater than zero (length in pulses): long note)
        /// </summary>
        public ulong l { get; set; }

        /// <summary>
        /// continuation flag
        /// </summary>
        public bool c { get; set; }
    }
}
