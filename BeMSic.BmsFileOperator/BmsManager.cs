using BeMSic.BmsFileOperator.LineOperation;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// BMS操作
    /// </summary>
    public class BmsManager
    {
        private readonly string _line;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">行</param>
        public BmsManager(string line)
        {
            _line = line;
        }

        /// <summary>
        /// MAIN行の#WAVインデックスをreplacesで置換する
        /// </summary>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>置換後BMSテキスト行</returns>
        public string ReplaceLineDefinition(List<BmsReplace> replaces)
        {
            var bmsLine = new BmsLine(_line);
            if (!bmsLine.IsMain())
            {
                return _line;
            }

            var mainLine = new MainLine(_line);
            return mainLine.ReplaceMainLineWav(replaces);
        }

        /// <summary>
        /// line内の#WAVインデックスをoffsetの分加算する
        /// </summary>
        /// <param name="wavs">#WAV番号一覧</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後のBMSテキスト行</returns>
        public string OffsettedLineDefinition(List<WavDefinition> wavs, int offset)
        {
            var bmsLine = new BmsLine(_line);
            if (bmsLine.IsMain())
            {
                var mainLine = new MainLine(_line);
                return mainLine.OffsetMainLineDefinition(wavs, offset);
            }

            if (bmsLine.IsWav())
            {
                var wavLine = new WavLine(_line);
                return wavLine.OffsetWavLineDefinition(wavs, offset);
            }

            // Copy line, if it is not to be replaced.
            return _line;
        }

        /// <summary>
        /// #WAVインデックスを詰めて並べる
        /// </summary>
        /// <param name="nowWavs">残す#WAV番号一覧</param>
        /// <returns>詰めた後のBMSテキスト行</returns>
        public string GetArrangedLine(List<WavDefinition> nowWavs)
        {
            List<BmsReplace> replaceList = new ();
            for (int i = 0; i < nowWavs.Count; i++)
            {
                var newWav = new WavDefinition(i + 1);
                replaceList.Add(new BmsReplace(nowWavs[i], newWav));
            }

            var bmsLine = new BmsLine(_line);
            if (bmsLine.IsMain())
            {
                var mainLine = new MainLine(_line);
                return mainLine.ReplaceMainLineWav(replaceList) + "\n";
            }

            if (bmsLine.IsWav())
            {
                var wavLine = new WavLine(_line);

                string retLine = wavLine.ReplaceWavLineDefinition(replaceList);
                if (retLine == string.Empty)
                {
                    // not defined
                    return string.Empty;
                }

                return retLine + "\n";
            }

            return _line + "\n";
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <returns>行内の#WAV番号一覧</returns>
        public List<WavDefinition> GetLineDefinition()
        {
            var bmsLine = new BmsLine(_line);
            if (!bmsLine.IsMain())
            {
                return new List<WavDefinition>();
            }

            var mainLine = new MainLine(_line);
            return mainLine.GetWavDefinition();
        }

        /// <summary>
        /// BGM行をoffsetの分ずらす
        /// </summary>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後の行</returns>
        public string ShiftBgmLine(int offset)
        {
            var bmsLine = new BmsLine(_line);
            if (!bmsLine.IsBgm())
            {
                return _line + "\n";
            }

            var mainLine = new MainLine(_line);
            return mainLine.ShiftBgmLane(offset) + "\n";
        }
    }
}
