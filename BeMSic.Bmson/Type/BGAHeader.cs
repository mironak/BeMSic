namespace BeMSic.Bmson.Type
{
    /// <summary>
    /// picture file
    /// </summary>
    public class BGAHeader
    {
        /// <summary>
        /// self-explanatory
        /// </summary>
        public ulong id { get; set; }

        /// <summary>
        /// picture file name
        /// </summary>
        public string name { get; set; } = string.Empty;
    }
}
