using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;
using System.Text.RegularExpressions;

namespace BeMSic.BmsFileOperator
{
    static public class BmsManager
    {
        /// <summary>
        /// BMSコマンド
        /// </summary>
        public enum BmsCommand
        {
            NONE,
            WAV,
            MAIN,
            MAIN_NOTOBJ,
        }

        /// <summary>
        /// Get #Wav definition in Main line
        /// </summary>
        class MainLineDef
        {
            public const int DataStart = 7;
            string _line;
            int _pos;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="line"></param>
            public MainLineDef(string line)
            {
                _line = line;
                _pos = DataStart;
            }

            public bool HasNext()
            {
                return (_pos < (_line.Length - 1));
            }

            public string Next()
            {
                var ret = _line.Substring(_pos, 2);
                _pos += 2;
                return ret;
            }
        }

        /// <summary>
        /// BMSファイルの1行のBMSコマンドを取得
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>BMSコマンド</returns>
        public static BmsCommand GetLineCommand(string line)
        {
            if (String.IsNullOrEmpty(line))
            {
                return BmsCommand.NONE;
            }

            if(line[0] != '#')
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
        /// Separate text on #WAV command line 
        /// "#WAV01 a.wav" -> ("01", "a.wav")
        /// </summary>
        /// <param name="line">line in .bms file</param>
        /// <returns>#WAV index, .wav file name</returns>
        public static (string, string) GetWavData(string line)
        {
            string[] arr = line.Split(new[] { ' ' }, 2);
            return (arr[0].Substring(4, 2), arr[1]);
        }

        /// <summary>
        /// Replace #WAV index in line to new #WAV index
        /// </summary>
        /// <param name="line"></param>
        /// <param name="originalWavFiles"></param>
        /// <param name="newWavFiles"></param>
        /// <returns></returns>
        public static string ReplaceLineDefinition(string line, List<BmsReplace> replaceList)
        {
            switch (GetLineCommand(line))
            {
                case BmsCommand.MAIN:
                    return ReplaceMainLineWav(line, replaceList);

                default:
                    // Copy line, if it is not to be replaced.
                    return line;
            }
        }

        /// <summary>
        /// Replace #WAV index in line to new #WAV index
        /// </summary>
        /// <param name="line"></param>
        /// <param name="originalWavFiles"></param>
        /// <param name="newWavFiles"></param>
        /// <returns></returns>
        public static string OffsettedLineDefinition(string line, List<int> wavList, int offset)
        {
            switch (GetLineCommand(line))
            {
                case BmsCommand.MAIN:
                    return OffsetMainLineDefinition(line, wavList, offset);

                case BmsCommand.WAV:
                    return OffsetWavLineDefinition(line, wavList, offset);

                default:
                    // Copy line, if it is not to be replaced.
                    return line;
            }
        }

        /// <summary>
        /// without any #WAV index gaps in line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="replacedTable"></param>
        /// <returns></returns>
        public static string ReductLineDefinition(string line, List<int> replacedTable)
        {
            List<BmsReplace> replaceList = new List<BmsReplace>();
            for (int i = 0; i < replacedTable.Count; i++)
            {
                replaceList.Add(new BmsReplace { NowDefinition = replacedTable[i], NewDefinition = i });
            }

            switch (GetLineCommand(line))
            {
                case BmsCommand.MAIN:
                    return (ReplaceMainLineWav(line, replaceList) + "\n");

                case BmsCommand.WAV:
                    string retLine = ReplaceWavLineDefinition(line, replaceList);
                    if (retLine == "")
                    {
                        // not defined
                        return "";
                    }
                    return (retLine + "\n");

                default:
                    return (line + "\n");
            }
        }

        /// <summary>
        /// Get line definition
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<int> GetLineDefinition(string line)
        {
            List<int> result = new List<int>();
            if (GetLineCommand(line) == BmsCommand.MAIN)
            {
                result = GetMainLineDefinition(line);
            }

            return result;
        }

        /// <summary>
        /// Replace #WAV index in Main line to new #WAV index
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        /// <param name="oldDef">Old #WAV List</param>
        /// <param name="newDef">New #WAV List</param>
        private static string OffsetMainLineDefinition(string line, List<int> wavList, int offset)
        {
            string retLine = line;
            retLine = OffsetMainLineWav(retLine, wavList, offset);
            return retLine;
        }

        /// <summary>
        /// Replace #WAV index in Main line to new #WAV index
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        /// <param name="oldDef">Old #WAV List</param>
        /// <param name="newDef">New #WAV List</param>
        private static string OffsetWavLineDefinition(string line, List<int> wavList, int offset)
        {
            var lineDefinition = RadixConvert.ZZToInt(line.Substring(4, 2));

            foreach (var replace in wavList)
            {
                // To go next wav, if it is not replaced.
                if (replace == lineDefinition)
                {
                    return $"#WAV{RadixConvert.IntToZZ(replace + offset)}{line.Substring(6)}";
                }
            }
            return "";
        }

        /// <summary>
        /// Replace #WAV index in Main line to new #WAV index
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        /// <param name="oldDef">Old #WAV List</param>
        /// <param name="newDef">New #WAV List</param>
        public static string ReplaceMainLineNumber(string line, int finalIndex)
        {
            int num;
            var success = int.TryParse(line.Substring(1, 3), out num);
            if (!success)
            {
                return "";
            }

            return $"#{(num + finalIndex).ToString("D3")}{line.Substring(4)}";
        }

        /// <summary>
        /// #WAV定義を置換する
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        /// <param name="replacedTable">replaced #WAV List</param>
        private static string ReplaceWavLineDefinition(string line, List<BmsReplace> replaces)
        {
            var nowWav = RadixConvert.ZZToInt(line.Substring(4, 2));
            var replace = replaces.Find(x => x.NowDefinition == nowWav);
            return $"#WAV{RadixConvert.IntToZZ(replace.NewDefinition)}{line.Substring(6)}";
        }

        /// <summary>
        /// Get main line definition
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        private static List<int> GetMainLineDefinition(string line)
        {
            List<int> ret = new List<int>();

            MainLineDef mainLine = new MainLineDef(line);
            while (mainLine.HasNext())
            {
                var writeVal = RadixConvert.ZZToInt(mainLine.Next());
                if (!ret.Contains(writeVal))
                {
                    ret.Add(writeVal);
                }
            }

            return ret;
        }

        /// <summary>
        /// #MAIN行の#WAV番号を置換する
        /// </summary>
        /// <param name="line">#MAIN行</param>
        /// <param name="replaces">置換テーブル</param>
        /// <returns></returns>
        private static string ReplaceMainLineWav(string line, List<BmsReplace> replaces)
        {
            MainLineDef mainLine = new MainLineDef(line);
            string dest = line.Substring(0, MainLineDef.DataStart);

            while (mainLine.HasNext())
            {
                var writeVal = mainLine.Next();

                foreach (var wav in replaces)
                {
                    if (RadixConvert.ZZToInt(writeVal) == wav.NowDefinition)
                    {
                        writeVal = RadixConvert.IntToZZ(wav.NewDefinition);
                        break;
                    }
                }
                dest += writeVal;
            }

            return dest;
        }

        /// <summary>
        /// #MAIN行の#WAV番号をoffsetの分増やす
        /// </summary>
        /// <param name="line">#MAIN行</param>
        /// <param name="wavs">#WAV一覧</param>
        /// <param name="offset">増分</param>
        /// <returns></returns>
        private static string OffsetMainLineWav(string line, List<int> wavs, int offset)
        {
            MainLineDef mainLine = new MainLineDef(line);
            string dest = line.Substring(0, MainLineDef.DataStart);

            while (mainLine.HasNext())
            {
                var writeVal = mainLine.Next();

                foreach(var wav in wavs)
                {
                    if (RadixConvert.ZZToInt(writeVal) == wav)
                    {
                        writeVal = RadixConvert.IntToZZ(wav + offset);
                        break;
                    }
                }
                dest += writeVal;
            }

            return dest;
        }

        /// <summary>
        /// lineが#WAV行ならtrue
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>lineが#WAV行ならtrue</returns>
        private static bool IsWavLine(string line)
        {
            Regex rgxWav = new Regex(@"^#WAV", RegexOptions.IgnoreCase);
            if (rgxWav.Match(line).Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// lineが#MAIN行ならtrue
        /// </summary>
        /// <param name="line">BMSファイルの1行</param>
        /// <returns>lineが#MAIN行ならtrue</returns>
        private static bool IsMainLine(string line)
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
        /// If line is MAIN, return true.
        /// </summary>
        /// <param name="line">.bms file text line</param>
        /// <returns></returns>
        private static bool IsMainNotObj(string line)
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
