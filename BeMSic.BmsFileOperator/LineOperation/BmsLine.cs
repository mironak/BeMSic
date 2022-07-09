using System.Text.RegularExpressions;
using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// BMSコマンド検索
    /// </summary>
    internal class BmsLine
    {
        private readonly string _line;
        private readonly BmsCommand _command;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">行</param>
        internal BmsLine(string line)
        {
            _line = line;
            _command = GetLineCommand();
        }

        /// <summary>
        /// BMSコマンド
        /// </summary>
        private enum BmsCommand
        {
            NONE,
            WAV,
            MAIN,
            MAIN_NOTOBJ,
        }

        /// <summary>
        /// 行が#WAVかを確認
        /// </summary>
        /// <returns>#WAVならtrue</returns>
        internal bool IsWav()
        {
            return _command == BmsCommand.WAV;
        }

        /// <summary>
        /// 行がMAINかを確認
        /// </summary>
        /// <returns>MAINならtrue</returns>
        internal bool IsMain()
        {
            return _command == BmsCommand.MAIN;
        }

        /// <summary>
        /// 行がMAIN(オブジェクト以外)かを確認
        /// </summary>
        /// <returns>#WAVならtrue</returns>
        internal bool IsMainNotObject()
        {
            return _command == BmsCommand.MAIN_NOTOBJ;
        }

        /// <summary>
        /// 行がBGMレーンかを確認
        /// </summary>
        /// <returns>BGMレーンならtrue</returns>
        internal bool IsBgm()
        {
            return IsBgmLine();
        }

        /// <summary>
        /// 同じ小節かを確認
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

        /// <summary>
        /// #WAVの番号を取得
        /// </summary>
        /// <returns>#WAV番号</returns>
        internal WavDefinition GetWavNumber()
        {
            return new WavDefinition(_line.Substring(4, 2));
        }

        /// <summary>
        /// 小節番号を取得
        /// </summary>
        /// <param name="result">小節番号戻り先</param>
        /// <returns>取得成功ならtrue</returns>
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
        /// 行がMAIN行(BGMか譜面レーン)かを確認
        /// </summary>
        /// <returns>行がMAIN行ならtrue</returns>
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
        /// 行がMAIN行(BGMか譜面レーン以外)かを確認
        /// </summary>
        /// <returns>行がMAIN行(BGMか譜面レーン以外)ならtrue</returns>
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
        /// 行が#WAV行かを確認
        /// </summary>
        /// <returns>行が#WAV行ならtrue</returns>
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
        /// 行がMAIN行(BGM)かを確認
        /// </summary>
        /// <returns>行がMAIN行(BGM)ならtrue</returns>
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

        /// <summary>
        /// 行がBGMかを確認
        /// </summary>
        /// <returns>BGMならtrue</returns>
        private bool IsBgmCommand()
        {
            if ((_line[4] == '0') && (_line[5] == '1'))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 行が譜面かを確認
        /// </summary>
        /// <returns>譜面ならtrue</returns>
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
