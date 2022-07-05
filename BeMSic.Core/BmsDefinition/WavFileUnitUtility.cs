using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// WAVファイル定義操作
    /// </summary>
    public static class WavFileUnitUtility
    {
        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="files">#WAV定義一覧</param>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        public static List<WavFileUnit> GetPartialWavs(List<WavFileUnit> files, int start, int end)
        {
            if (!IsInRange(start, end))
            {
                throw new ArgumentOutOfRangeException(nameof(start) + "and" + nameof(end), "Not in range");
            }

            return GetPartialWavsCore(files, start, end);
        }

        /// <summary>
        /// #WAVインデックスが"01"から"ZZ"の範囲ならtrue
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>startとendが"01"から"ZZ"の範囲ならtrue</returns>
        private static bool IsInRange(int start, int end)
        {
            if (end <= start)
            {
                return false;
            }

            if (start < 1)
            {
                return false;
            }

            if (RadixConvert.ZZToInt("ZZ") < end)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="fileList">#WAV定義一覧</param>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        private static List<WavFileUnit> GetPartialWavsCore(List<WavFileUnit> fileList, int start, int end)
        {
            List<WavFileUnit> partialWavs = new ();
            foreach (WavFileUnit wav in fileList)
            {
                if (wav.Wav.Num < start)
                {
                    continue;
                }

                if (end < wav.Wav.Num)
                {
                    break;
                }

                partialWavs.Add(wav);
            }

            return partialWavs;
        }
    }
}
