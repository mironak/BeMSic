using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// #WAV行操作
    /// </summary>
    internal class WavLine
    {
        private string _line;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">行</param>
        public WavLine(string line)
        {
            _line = line;
        }

        /// <summary>
        /// #WAV行の#WAVインデックスをoffsetの分加算した行を返す
        /// </summary>
        /// <param name="wavs">#WAV番号一覧</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後の#WAV行</returns>
        internal string OffsetWavLineDefinition(WavDefinitions wavs, int offset)
        {
            WavDefinition lineDefinition = new WavDefinition(_line.Substring(4, 2));

            if (wavs.Contains(lineDefinition))
            {
                var offsetedWav = new WavDefinition(lineDefinition.Num + offset);
                return $"#WAV{offsetedWav.ZZ}{_line[6..]}";
            }

            return string.Empty;
        }

        /// <summary>
        /// #WAV定義をreplacesで置換する
        /// </summary>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>置換後#WAV行</returns>
        internal string ReplaceWavLineDefinition(List<BmsReplace> replaces)
        {
            WavDefinition nowWav = new WavDefinition(RadixConvert.ZZToInt(_line.Substring(4, 2)));
            BmsReplace? replace = replaces.Find(x => x.NowNum.Num == nowWav.Num);
            if (replace == null)
            {
                return _line;
            }

            return $"#WAV{replace.NewNum.ZZ}{_line[6..]}";
        }

        /// <summary>
        /// #WAVコマンド行から#WAVインデックスとwavファイル名を取得する
        /// </summary>
        /// <returns>#WAVデータ</returns>
        internal WavFileUnit GetWavData()
        {
            string[] arr = _line.Split(new[] { ' ' }, 2);
            return new WavFileUnit(RadixConvert.ZZToInt(arr[0].Substring(4, 2)), arr[1]);
        }
    }
}
