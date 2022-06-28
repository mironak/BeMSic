namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// bga note
    /// </summary>
    public class BGAEvent
    {
        /// <summary>
        /// pulse number
        /// </summary>
        public ulong y { get; set; }

        /// <summary>
        /// corresponds to BGAHeader.id
        /// </summary>
        public ulong id { get; set; }
    }
}
