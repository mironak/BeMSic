using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator.LineOperation
{
    /// <summary>
    /// #WAV行操作
    /// </summary>
    internal class WavLineManager
    {
        /// <summary>
        /// #WAV行の#WAVインデックスをoffsetの分加算した行を返す
        /// </summary>
        /// <param name="line">#WAV行</param>
        /// <param name="wavs">#WAV番号一覧</param>
        /// <param name="offset">ずらす数</param>
        /// <returns>ずらした後の#WAV行</returns>
        internal static string OffsetWavLineDefinition(string line, List<int> wavs, int offset)
        {
            int lineDefinition = RadixConvert.ZZToInt(line.Substring(4, 2));

            foreach (int replace in wavs)
            {
                // To go next wav, if it is not replaced.
                if (replace == lineDefinition)
                {
                    return $"#WAV{RadixConvert.IntToZZ(replace + offset)}{line[6..]}";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// #WAV定義をreplacesで置換する
        /// </summary>
        /// <param name="line">#WAV行</param>
        /// <param name="replaces">置換テーブル</param>
        /// <returns>置換後#WAV行</returns>
        internal static string ReplaceWavLineDefinition(string line, List<BmsReplace> replaces)
        {
            int nowWav = RadixConvert.ZZToInt(line.Substring(4, 2));
            BmsReplace? replace = replaces.Find(x => x.NowNum == nowWav);
            if (replace == null)
            {
                return line;
            }

            return $"#WAV{RadixConvert.IntToZZ(replace.NewNum)}{line[6..]}";
        }

        /// <summary>
        /// #WAVコマンド行から#WAVインデックスとwavファイル名を取得する
        /// </summary>
        /// <param name="line">#WAV行</param>
        /// <returns>#WAVデータ</returns>
        internal static WavFileUnit GetWavData(string line)
        {
            string[] arr = line.Split(new[] { ' ' }, 2);
            return new WavFileUnit(RadixConvert.ZZToInt(arr[0].Substring(4, 2)), arr[1]);
        }
    }
}
