using System.Text.RegularExpressions;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// BMSコマンド検索
    /// </summary>
    internal static class BmsCommandSearch
    {
        /// <summary>
        /// BMSコマンド
        /// </summary>
        internal enum BmsCommand
        {
            NONE,
            WAV,
            MAIN,
            MAIN_NOTOBJ,
        }

        /// <summary>
        /// BMSファイルの1行のBMSコマンドを取得
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>BMSコマンド</returns>
        internal static BmsCommand GetLineCommand(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return BmsCommand.NONE;
            }

            if (line[0] != '#')
            {
                return BmsCommand.NONE;
            }

            if (IsWavLine(line))
            {
                return BmsCommand.WAV;
            }

            if (IsMainLine(line))
            {
                return BmsCommand.MAIN;
            }

            if (IsMainNotObj(line))
            {
                return BmsCommand.MAIN_NOTOBJ;
            }

            return BmsCommand.NONE;
        }

        /// <summary>
        /// lineが#WAV行ならtrue
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>lineが#WAV行ならtrue</returns>
        internal static bool IsWavLine(string line)
        {
            Regex rgxWav = new (@"^#WAV", RegexOptions.IgnoreCase);
            if (rgxWav.Match(line).Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// lineがMAIN行(BGMか譜面レーン)ならtrue
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>lineがMAIN行ならtrue</returns>
        internal static bool IsMainLine(string line)
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(line).Success)
            {
                if (IsBgmCommand(line))
                {
                    return true;
                }

                if (IsPatternCommand(line))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// lineがMAIN行(BGMか譜面レーン以外)ならtrueを返す
        /// </summary>
        /// <param name="line">.bms file text line</param>
        /// <returns>lineがMAIN行(BGMか譜面レーン以外)ならtrue</returns>
        internal static bool IsMainNotObj(string line)
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(line).Success)
            {
                if (IsBgmCommand(line))
                {
                    return false;
                }

                if (IsPatternCommand(line))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// lineがMAIN行(BGM)ならtrueを返す
        /// </summary>
        /// <param name="line">.bms file text line</param>
        /// <returns>lineがMAIN行(BGMか譜面レーン以外)ならtrue</returns>
        internal static bool IsBgmLine(string line)
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(line).Success)
            {
                // BGM lane
                if (IsBgmCommand(line))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 同じ小節ならtrue
        /// </summary>
        /// <param name="line1">.bms file text line1</param>
        /// <param name="line2">.bms file text line2</param>
        /// <returns>同じ小節ならtrue</returns>
        internal static bool IsSameBar(string line1, string line2)
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (!rgxMain.Match(line1).Success)
            {
                return false;
            }

            if (!rgxMain.Match(line2).Success)
            {
                return false;
            }

            if (line1[1] != line2[1] || line1[2] != line2[2] || line1[3] != line2[3])
            {
                return false;
            }

            return true;
        }

        private static bool IsBgmCommand(string line)
        {
            if ((line[4] == '0') && (line[5] == '1'))
            {
                return true;
            }

            return false;
        }

        private static bool IsPatternCommand(string line)
        {
            if ((line[4] == '1') || (line[4] == '2') || (line[4] == '5') || (line[4] == '6'))
            {
                return true;
            }

            return false;
        }
    }
}
