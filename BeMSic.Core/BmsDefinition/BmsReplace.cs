namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// BMS置換前後#WAV番号
    /// </summary>
    public class BmsReplace
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nowNum">#WAV番号</param>
        /// <param name="newNum">ファイル名</param>
        public BmsReplace(WavDefinition nowNum, WavDefinition newNum)
        {
            NowNum = nowNum;
            NewNum = newNum;
        }

        /// <summary>
        /// 現在の#WAV番号
        /// </summary>
        public WavDefinition NowNum { get; set; }

        /// <summary>
        /// 新しい#WAV番号
        /// </summary>
        public WavDefinition NewNum { get; set; }
    }
}
