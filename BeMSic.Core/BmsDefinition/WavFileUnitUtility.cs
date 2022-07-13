using System.Collections.Immutable;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// WAVファイル定義操作
    /// </summary>
    public class WavFileUnitUtility
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WavFileUnitUtility()
        {
            Files = new List<WavFileUnit>();
        }

        public List<WavFileUnit> Files { get; }

        /// <summary>
        /// WAVファイル情報追加
        /// </summary>
        /// <param name="num">#WAV番号</param>
        /// <param name="name">ファイル名</param>
        public void Add(int num, string name)
        {
            Files.Add(new WavFileUnit(num, name));
        }

        /// <summary>
        /// WAVファイル情報追加
        /// </summary>
        /// <param name="wav">#WAV情報</param>
        public void Add(WavFileUnit wav)
        {
            Files.Add(wav);
        }

        /// <summary>
        /// ファイル数取得
        /// </summary>
        /// <returns>ファイル数</returns>
        public int Count()
        {
            return Files.Count;
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        public List<WavFileUnit> GetPartialWavs(WavDefinition start, WavDefinition end)
        {
            if (!IsInRange(start, end))
            {
                throw new ArgumentOutOfRangeException(nameof(start) + "and" + nameof(end), "Not in range");
            }

            return GetPartialWavsCore(start, end);
        }

        /// <summary>
        /// #WAVインデックスが"01"から"ZZ"の範囲ならtrue
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>startとendが"01"から"ZZ"の範囲ならtrue</returns>
        private static bool IsInRange(WavDefinition start, WavDefinition end)
        {
            if (end.Num <= start.Num)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get partial wav file list
        /// </summary>
        /// <param name="start">#WAV定義開始番号</param>
        /// <param name="end">#WAV定義最終番号</param>
        /// <returns>#WAV定義一覧(startからendまで)</returns>
        private List<WavFileUnit> GetPartialWavsCore(WavDefinition start, WavDefinition end)
        {
            List<WavFileUnit> partialWavs = new ();

            foreach (WavFileUnit wav in Files)
            {
                if (wav.Wav.Num < start.Num)
                {
                    continue;
                }

                if (end.Num < wav.Wav.Num)
                {
                    break;
                }

                partialWavs.Add(wav);
            }

            return partialWavs;
        }
    }
}
