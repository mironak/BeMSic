namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// Wav files information
    /// </summary>
    public class WavFileUnit
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="name">ファイル名</param>
        public WavFileUnit(int num, string name)
        {
            Num = num;
            Name = name;
        }

        /// <summary>
        /// #WAV番号
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string Name { get; set; }
    }
}
