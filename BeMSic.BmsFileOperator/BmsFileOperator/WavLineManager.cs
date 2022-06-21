using BeMSic.Core.BmsDefinition;
using BeMSic.Core.Helpers;

namespace BeMSic.BmsFileOperator
{
    internal class WavLineManager
    {
        /// <summary>
        /// #WAV行の#WAVインデックスをoffsetの分加算した行を返す
        /// </summary>
        /// <param name="line"></param>
        /// <param name="wavs"></param>
        /// <param name="offset"></param>
        internal static string OffsetWavLineDefinition(string line, List<int> wavs, int offset)
        {
            var lineDefinition = RadixConvert.ZZToInt(line.Substring(4, 2));

            foreach (var replace in wavs)
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
        /// #WAV定義をreplacesで置換する
        /// </summary>
        /// <param name="line"></param>
        /// <param name="replaces"></param>
        internal static string ReplaceWavLineDefinition(string line, List<BmsReplace> replaces)
        {
            var nowWav = RadixConvert.ZZToInt(line.Substring(4, 2));
            var replace = replaces.Find(x => x.NowDefinition == nowWav);
            return $"#WAV{RadixConvert.IntToZZ(replace.NewDefinition)}{line.Substring(6)}";
        }

        /// <summary>
        /// #WAVコマンド行から#WAVインデックスとwavファイル名を取得する
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        internal static WavFileUnit GetWavData(string line)
        {
            string[] arr = line.Split(new[] { ' ' }, 2);
            return new WavFileUnit(RadixConvert.ZZToInt(arr[0].Substring(4, 2)), arr[1]);
        }
    }
}
