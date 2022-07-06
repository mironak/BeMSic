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
        internal static List<WavDefinition> GetWavDefinition(string line)
        {
            List<WavDefinition> result = new ();
            MainLineDef mainLine = new (line);

            while (mainLine.HasNext())
            {
                var next = mainLine.Next();

                // 00は無視
                if (next == "00")
                {
                    continue;
                }

                var writeWav = new WavDefinition(next);
                if (!result.Contains(writeWav))
                {
                    result.Add(writeWav);
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
            MainLineDef mainLine = new MainLineDef(line);
            string dest = line[..MainLineDef.DataStart];

            while (mainLine.HasNext())
            {
                var next = mainLine.Next();

                // 00は無視
                if (next == "00")
                {
                    dest += next;
                    continue;
                }

                var writeVal = new WavDefinition(next);

                foreach (BmsReplace wav in replaces)
                {
                    if (writeVal.Equals(wav.NowNum))
                    {
                        writeVal = wav.NewNum;
                        break;
                    }
                }

                dest += writeVal.ZZ;
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
        internal static string OffsetMainLineDefinition(string line, List<WavDefinition> wavs, int offset)
        {
            List<BmsReplace> offsetedWavs = new ();
            foreach (var wav in wavs)
            {
                offsetedWavs.Add(new BmsReplace(wav, new WavDefinition(wav.Num + offset)));
            }

            return ReplaceMainLineWav(line, offsetedWavs);
        }

        /// <summary>
        /// BGM行をoffsetの分右にずらす
        /// </summary>
        /// <param name="line">#MAIN行</param>
        /// <param name="offset">増分</param>
        /// <returns>#MAIN行(ずらし後)</returns>
        internal static string ShiftBgmLane(string line, int offset)
        {
            string dest = string.Empty;
            string destHead = line[..MainLineDef.DataStart];

            for (int i = 0; i < offset; i++)
            {
                dest += destHead + "00\n";
            }

            dest += line;

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
                var ret = _line.Substring(_pos, 2);
                _pos += 2;
                return ret;
            }
        }
    }
}
