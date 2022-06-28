namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// bga
    /// </summary>
    public class BGA
    {
        /// <summary>
        /// picture id and filename
        /// </summary>
        public BGAHeader[]? bga_header { get; set; }

        /// <summary>
        /// picture sequence
        /// </summary>
        public BGAEvent[]? bga_events { get; set; }

        /// <summary>
        /// picture sequence overlays bga_notes
        /// </summary>
        public BGAEvent[]? layer_events { get; set; }

        /// <summary>
        /// picture sequence when missed
        /// </summary>
        public BGAEvent[]? poor_events { get; set; }
    }
}
