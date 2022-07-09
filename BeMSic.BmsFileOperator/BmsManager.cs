using BeMSic.BmsFileOperator.LineOperation;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator
{
    /// <summary>
    /// BMS操作
    /// </summary>
    public static class BmsManager
    {
        /// <summary>
        /// MAIN行の#WAVインデックスをreplacesで置換する
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>置換後BMSテキスト行</returns>
        public static string ReplaceLineDefinition(string line, List<BmsReplace> replaces)
        {
            var bmsLine = new BmsLine(line);
            if (!bmsLine.IsMain())
            {
                return line;
            }

            return MainLineManager.ReplaceMainLineWav(line, replaces);
        }

        /// <summary>
        /// line内の#WAVインデックスをoffsetの分加算する
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <param name="wavs">#WAV番号一覧</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後のBMSテキスト行</returns>
        public static string OffsettedLineDefinition(string line, List<WavDefinition> wavs, int offset)
        {
            var bmsLine = new BmsLine(line);
            if (bmsLine.IsMain())
            {
                return MainLineManager.OffsetMainLineDefinition(line, wavs, offset);
            }

            if (bmsLine.IsWav())
            {
                return WavLineManager.OffsetWavLineDefinition(line, wavs, offset);
            }

            // Copy line, if it is not to be replaced.
            return line;
        }

        /// <summary>
        /// #WAVインデックスを詰めて並べる
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <param name="nowWavs">残す#WAV番号一覧</param>
        /// <returns>詰めた後のBMSテキスト行</returns>
        public static string GetArrangedLine(string line, List<WavDefinition> nowWavs)
        {
            List<BmsReplace> replaceList = new ();
            for (int i = 0; i < nowWavs.Count; i++)
            {
                var newWav = new WavDefinition(i + 1);
                replaceList.Add(new BmsReplace(nowWavs[i], newWav));
            }

            var bmsLine = new BmsLine(line);
            if (bmsLine.IsMain())
            {
                return MainLineManager.ReplaceMainLineWav(line, replaceList) + "\n";
            }

            if (bmsLine.IsWav())
            {
                string retLine = WavLineManager.ReplaceWavLineDefinition(line, replaceList);
                if (retLine == string.Empty)
                {
                    // not defined
                    return string.Empty;
                }

                return retLine + "\n";
            }

            return line + "\n";
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <returns>行内の#WAV番号一覧</returns>
        public static List<WavDefinition> GetLineDefinition(string line)
        {
            var bmsLine = new BmsLine(line);
            if (!bmsLine.IsMain())
            {
                return new List<WavDefinition>();
            }

            return MainLineManager.GetWavDefinition(line);
        }

        public static string ShiftBgmLine(string line, int offset)
        {
            var bmsLine = new BmsLine(line);
            if (!bmsLine.IsBgm())
            {
                return line + "\n";
            }

            return MainLineManager.ShiftBgmLane(line, offset) + "\n";
        }
    }
}
