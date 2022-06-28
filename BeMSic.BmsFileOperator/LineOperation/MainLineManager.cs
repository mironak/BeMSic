﻿using BeMSic.Core.BmsDefinition;
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
            int num;
            var success = int.TryParse(line.Substring(1, 3), out num);
            if (!success)
            {
                return string.Empty;
            }

            return $"#{(num + offset).ToString("D3")}{line.Substring(4)}";
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <param name="line">MAIN行</param>
        /// <returns>#WAV定義一覧</returns>
        internal static List<int> GetWavDefinition(string line)
        {
            List<int> result = new List<int>();
            MainLineDef mainLine = new MainLineDef(line);
            while (mainLine.HasNext())
            {
                var writeVal = RadixConvert.ZZToInt(mainLine.Next());
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
            MainLineDef mainLine = new MainLineDef(line);
            string dest = line.Substring(0, MainLineDef.DataStart);

            while (mainLine.HasNext())
            {
                var writeVal = mainLine.Next();

                foreach (var wav in replaces)
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
            string retLine = line;
            MainLineDef mainLine = new MainLineDef(retLine);
            string dest = retLine.Substring(0, MainLineDef.DataStart);

            while (mainLine.HasNext())
            {
                var writeVal = mainLine.Next();

                foreach (var wav in wavs)
                {
                    if (RadixConvert.ZZToInt(writeVal) == wav)
                    {
                        writeVal = RadixConvert.IntToZZ(wav + offset);
                        break;
                    }
                }

                dest += writeVal;
            }

            return retLine;
        }

        /// <summary>
        /// MAIN行の#WAVインデックスを1つずつ取り出す
        /// </summary>
        private class MainLineDef
        {
            public const int DataStart = 7;
            private string _line;
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

            public bool HasNext()
            {
                return _pos < (_line.Length - 1);
            }

            public string Next()
            {
                var ret = _line.Substring(_pos, 2);
                _pos += 2;
                return ret;
            }
        }
    }
}