using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// MAIN行操作
    /// </summary>
    internal static class MainLineManager
    {
        /// <summary>
        /// 小節番号をoffsetの分後ろにずらす
        /// </summary>
        /// <param name="line">Line in .bms file</param>
        /// <param name="offset">Old #WAV List</param>
        /// <returns>小節番号をずらした行</returns>
        internal static string OffsetMainLineBar(string line, int offset)
        {
            bool success = int.TryParse(line.AsSpan(1, 3), out int num);
            if (!success)
            {
                return string.Empty;
            }

            return $"#{num + offset:D3}{line[4..]}";
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <param name="line">MAIN行</param>
        /// <returns>#WAV定義一覧</returns>
        internal static List<int> GetWavDefinition(string line)
        {
            List<int> result = new ();
            MainLineDef mainLine = new (line);
            while (mainLine.HasNext())
            {
                int writeVal = RadixConvert.ZZToInt(mainLine.Next());

                // 00は無視
                if (writeVal == 0)
                {
                    continue;
                }

                if (!result.Contains(writeVal))
                {
                    result.Add(writeVal);
                }
            }

            return result;
        }

        /// <summary>
        /// #MAIN行の#WAV番号を置換する
        /// </summary>
        /// <param name="line">#MAIN行</param>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>#MAIN行(#WAV置換後)</returns>
        internal static string ReplaceMainLineWav(string line, List<BmsReplace> replaces)
        {
            MainLineDef mainLine = new (line);
            string dest = line[..MainLineDef.DataStart];

            while (mainLine.HasNext())
            {
                string writeVal = mainLine.Next();

                foreach (BmsReplace wav in replaces)
                {
                    if (RadixConvert.ZZToInt(writeVal) == wav.NowNum)
                    {
                        writeVal = RadixConvert.IntToZZ(wav.NewNum);
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
        /// <returns>#MAIN行(#WAVずらし後)</returns>
        internal static string OffsetMainLineDefinition(string line, List<int> wavs, int offset)
        {
            MainLineDef mainLine = new (line);
            string dest = line[..MainLineDef.DataStart];

            while (mainLine.HasNext())
            {
                string writeVal = mainLine.Next();

                foreach (int wav in wavs)
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
        /// MAIN行の#WAVインデックスを1つずつ取り出す
        /// </summary>
        private class MainLineDef
        {
            public const int DataStart = 7;
            private readonly string _line;
            private int _pos;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="line">MAIN行</param>
            public MainLineDef(string line)
            {
                _line = line;
                _pos = DataStart;
            }

            /// <summary>
            /// 次の#WAV番号を確認
            /// </summary>
            /// <returns>次の#WAV番号があればtrue</returns>
            public bool HasNext()
            {
                return _pos < (_line.Length - 1);
            }

            /// <summary>
            /// 次の#WAV番号を取得
            /// </summary>
            /// <returns>#WAV番号</returns>
            public string Next()
            {
                string ret = _line.Substring(_pos, 2);
                _pos += 2;
                return ret;
            }
        }
    }
}
