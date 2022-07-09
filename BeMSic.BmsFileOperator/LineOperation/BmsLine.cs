using BeMSic.Core.BmsDefinition;
using System.Text.RegularExpressions;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// BMSコマンド検索
    /// </summary>
    internal class BmsLine
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

        private string _line;
        internal readonly BmsCommand Command;

        internal BmsLine(string line)
        {
            _line = line;
            Command = GetLineCommand();
        }

        internal bool IsWav()
        {
            return Command == BmsCommand.WAV;
        }

        internal bool IsMain()
        {
            return Command == BmsCommand.MAIN;
        }

        internal bool IsMainNotObject()
        {
            return IsMainNotObj();
        }

        internal bool IsBgm()
        {
            return IsBgmLine();
        }

        /// <summary>
        /// 同じ小節ならtrue
        /// </summary>
        /// <param name="otherLine">.bms file text line</param>
        /// <returns>同じ小節ならtrue</returns>
        internal bool IsSameBar(BmsLine otherLine)
        {
            int line1;
            if (!this.TryGetBar(out line1))
            {
                return false;
            }

            int line2;
            if (!otherLine.TryGetBar(out line2))
            {
                return false;
            }

            if (line1 != line2)
            {
                return false;
            }

            return true;
        }

        internal WavDefinition GetCommandNumber()
        {
            return new WavDefinition(_line.Substring(4, 2));
        }

        /// <summary>
        /// 小節番号を取得
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal bool TryGetBar(out int result)
        {
            result = 0;

            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (!rgxMain.Match(_line).Success)
            {
                return false;
            }

            int.TryParse(_line.Substring(1, 3), out result);
            return true;
        }

        /// <summary>
        /// BMSファイルの1行のBMSコマンドを取得
        /// </summary>
        /// <returns>BMSコマンド</returns>
        private BmsCommand GetLineCommand()
        {
            if (string.IsNullOrEmpty(_line))
            {
                return BmsCommand.NONE;
            }

            if (_line[0] != '#')
            {
                return BmsCommand.NONE;
            }

            if (IsWavLine())
            {
                return BmsCommand.WAV;
            }

            if (IsMainLine())
            {
                return BmsCommand.MAIN;
            }

            if (IsMainNotObj())
            {
                return BmsCommand.MAIN_NOTOBJ;
            }

            return BmsCommand.NONE;
        }

        /// <summary>
        /// lineがMAIN行(BGMか譜面レーン)ならtrue
        /// </summary>
        /// <returns>lineがMAIN行ならtrue</returns>
        private bool IsMainLine()
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(_line).Success)
            {
                if (IsBgmCommand())
                {
                    return true;
                }

                if (IsPatternCommand())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// lineがMAIN行(BGMか譜面レーン以外)ならtrueを返す
        /// </summary>
        /// <returns>lineがMAIN行(BGMか譜面レーン以外)ならtrue</returns>
        private bool IsMainNotObj()
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(_line).Success)
            {
                if (IsBgmCommand())
                {
                    return false;
                }

                if (IsPatternCommand())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// lineが#WAV行ならtrue
        /// </summary>
        /// <returns>lineが#WAV行ならtrue</returns>
        private bool IsWavLine()
        {
            Regex rgxWav = new (@"^#WAV", RegexOptions.IgnoreCase);
            if (rgxWav.Match(_line).Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// lineがMAIN行(BGM)ならtrueを返す
        /// </summary>
        /// <returns>lineがMAIN行(BGMか譜面レーン以外)ならtrue</returns>
        private bool IsBgmLine()
        {
            Regex rgxMain = new (@"^#[0-9][0-9][0-9]", RegexOptions.IgnoreCase);
            if (rgxMain.Match(_line).Success)
            {
                // BGM lane
                if (IsBgmCommand())
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsBgmCommand()
        {
            if ((_line[4] == '0') && (_line[5] == '1'))
            {
                return true;
            }

            return false;
        }

        private bool IsPatternCommand()
        {
            if (_line[4] is '1' or '2' or '5' or '6')
            {
                return true;
            }

            return false;
        }
    }
}
