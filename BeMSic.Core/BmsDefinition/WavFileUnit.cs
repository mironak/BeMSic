namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// Wav files information
    /// </summary>
    public class WavFileUnit
    {
        /// <summary>
        /// Index
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="num"></param>
        /// <param name="name"></param>
        public WavFileUnit(int num, string name)
        {
            Num = num;
            Name = name;
        }
    }

    /// <summary>
    /// BMS置換前後#WAV番号
    /// </summary>
    public struct BmsReplace
    {
        public int NowDefinition;
        public int NewDefinition;
    }

}
