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
            switch (BmsCommandSearch.GetLineCommand(line))
            {
                case BmsCommandSearch.BmsCommand.MAIN:
                    return MainLineManager.ReplaceMainLineWav(line, replaces);

                default:
                    // Copy line, if it is not to be replaced.
                    return line;
            }
        }

        /// <summary>
        /// line内の#WAVインデックスをoffsetの分加算する
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <param name="wavs">#WAV番号一覧</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後のBMSテキスト行</returns>
        public static string OffsettedLineDefinition(string line, List<int> wavs, int offset)
        {
            switch (BmsCommandSearch.GetLineCommand(line))
            {
                case BmsCommandSearch.BmsCommand.MAIN:
                    return MainLineManager.OffsetMainLineDefinition(line, wavs, offset);

                case BmsCommandSearch.BmsCommand.WAV:
                    return WavLineManager.OffsetWavLineDefinition(line, wavs, offset);

                default:
                    // Copy line, if it is not to be replaced.
                    return line;
            }
        }

        /// <summary>
        /// #WAVインデックスを詰めて並べる
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <param name="nowWavs">残す#WAV番号一覧</param>
        /// <returns>詰めた後のBMSテキスト行</returns>
        public static string GetArrangedLine(string line, List<int> nowWavs)
        {
            List<BmsReplace> replaceList = new List<BmsReplace>();
            for (int i = 0; i < nowWavs.Count; i++)
            {
                replaceList.Add(new BmsReplace(nowWavs[i], i));
            }

            switch (BmsCommandSearch.GetLineCommand(line))
            {
                case BmsCommandSearch.BmsCommand.MAIN:
                    return MainLineManager.ReplaceMainLineWav(line, replaceList) + "\n";

                case BmsCommandSearch.BmsCommand.WAV:
                    string retLine = WavLineManager.ReplaceWavLineDefinition(line, replaceList);
                    if (retLine == string.Empty)
                    {
                        // not defined
                        return string.Empty;
                    }

                    return retLine + "\n";

                default:
                    return line + "\n";
            }
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <param name="line">BMSテキスト行</param>
        /// <returns>行内の#WAV番号一覧</returns>
        public static List<int> GetLineDefinition(string line)
        {
            if (BmsCommandSearch.GetLineCommand(line) != BmsCommandSearch.BmsCommand.MAIN)
            {
                return new List<int>();
            }

            return MainLineManager.GetWavDefinition(line);
        }
    }
}
