using BeMSic.Core.BmsDefinition;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// MAIN行操作
    /// </summary>
    internal class MainLine
    {
        private string _line;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="line">行</param>
        public MainLine(string line)
        {
            _line = line;
        }

        /// <summary>
        /// 小節番号をoffsetの分後ろにずらす
        /// </summary>
        /// <param name="offset">Old #WAV List</param>
        /// <returns>小節番号をずらした行</returns>
        internal string OffsetMainLineBar(int offset)
        {
            bool success = int.TryParse(_line.AsSpan(1, 3), out int num);
            if (!success)
            {
                return string.Empty;
            }

            return $"#{num + offset:D3}{_line[4..]}";
        }

        /// <summary>
        /// MAIN行1行に含まれる#WAV定義一覧を返す
        /// </summary>
        /// <returns>#WAV定義一覧</returns>
        internal List<WavDefinition> GetWavDefinition()
        {
            List<WavDefinition> result = new ();
            MainDefinitionReader mainLine = new (_line);

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
        /// <param name="replaces">置換テーブル</param>
        /// <returns>#MAIN行(#WAV置換後)</returns>
        internal string ReplaceMainLineWav(List<BmsReplace> replaces)
        {
            var mainLine = new MainDefinitionReader(_line);
            string dest = _line[..MainDefinitionReader.DataStart];

            while (mainLine.HasNext())
            {
                var next = mainLine.Next();
                dest += GetReplacedDefinition(next, replaces);
            }

            return dest;
        }

        /// <summary>
        /// #MAIN行の#WAV番号をoffsetの分増やす
        /// </summary>
        /// <param name="wavs">#WAV一覧</param>
        /// <param name="offset">増分</param>
        /// <returns>#MAIN行(#WAVずらし後)</returns>
        internal string OffsetMainLineDefinition(List<WavDefinition> wavs, int offset)
        {
            List<BmsReplace> offsetedWavs = new ();
            foreach (var wav in wavs)
            {
                offsetedWavs.Add(new BmsReplace(wav, new WavDefinition(wav.Num + offset)));
            }

            return ReplaceMainLineWav(offsetedWavs);
        }

        /// <summary>
        /// BGM行をoffsetの分右にずらす
        /// </summary>
        /// <param name="offset">増分</param>
        /// <returns>#MAIN行(ずらし後)</returns>
        internal string ShiftBgmLane(int offset)
        {
            string dest = string.Empty;
            string destHead = _line[..MainDefinitionReader.DataStart];

            for (int i = 0; i < offset; i++)
            {
                dest += destHead + "00\n";
            }

            dest += _line;

            return dest;
        }

        private static string GetReplacedDefinition(string def, List<BmsReplace> replaces)
        {
            // 00は無視
            if (def == "00")
            {
                return def;
            }

            var writeVal = new WavDefinition(def);

            foreach (BmsReplace wav in replaces)
            {
                if (writeVal.Equals(wav.NowNum))
                {
                    writeVal = wav.NewNum;
                    break;
                }
            }

            return writeVal.ZZ;
        }
    }
}
