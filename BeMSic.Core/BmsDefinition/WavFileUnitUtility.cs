using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    public static class WavFileUnitUtility
    {
        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="files"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<WavFileUnit> GetPartialWavList(List<WavFileUnit> files, int start, int end)
        {
            if (!IsInRange(start, end))
            {
                throw new ArgumentOutOfRangeException();
            }

            return GetPartialWavs(files, start, end);
        }

        /// <summary>
        /// #WAVインデックスが"01"から"ZZ"の範囲ならtrue
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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
        /// <param name="fileList"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static List<WavFileUnit> GetPartialWavs(List<WavFileUnit> fileList, int start, int end)
        {
            List<WavFileUnit> partialWavs = new List<WavFileUnit>();
            foreach (var wav in fileList)
            {
                if (wav.Num < start)
                {
                    continue;
                }
                if (end < wav.Num)
                {
                    break;
                }
                partialWavs.Add(wav);
            }
            return partialWavs;
        }
    }
}
