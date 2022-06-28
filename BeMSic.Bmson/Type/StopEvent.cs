namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// stop note
    /// </summary>
    public class StopEvent
    {
        /// <summary>
        /// pulse number
        /// </summary>
        public ulong y { get; set; }

        /// <summary>
        /// stop duration (pulses to stop)
        /// </summary>
        public ulong duration { get; set; }
    }
}
