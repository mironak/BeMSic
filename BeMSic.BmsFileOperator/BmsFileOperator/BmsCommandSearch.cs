using System.Text.RegularExpressions;

namespace BeMSic.BmsFileOperator
{
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
            if (String.IsNullOrEmpty(line))
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
            Regex rgxWav = new Regex(@"^#WAV", RegexOptions.IgnoreCase);
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
            Regex rgxMain = new Regex(@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(line).Success)
            {
                // BGM lane
                if ((line[4] == '0') && (line[5] == '1'))
                {
                    return true;
                }

                // Pettern lane
                if ((line[4] == '1') || (line[4] == '2'))
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
        /// <returns></returns>
        internal static bool IsMainNotObj(string line)
        {
            Regex rgxMain = new Regex(@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(line).Success)
            {
                // BGM lane
                if ((line[4] == '0') && (line[5] == '1'))
                {
                    return false;
                }

                // Pettern lane
                if ((line[4] == '1') || (line[4] == '2'))
                {
                    return false;
                }

                return true;
            }
            return false;
        }
    }
}
