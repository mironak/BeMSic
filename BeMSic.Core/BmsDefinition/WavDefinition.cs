using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    public class WavDefinition
    {
        public readonly int Num;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="num">#WAVインデックス</param>
        public WavDefinition(int num)
        {
            if (!IsInRange(num))
            {
                throw new ArgumentOutOfRangeException(nameof(num));
            }

            Num = num;
        }

        /// <summary>
        /// #WAVインデックスが"01"から"ZZ"の範囲ならtrue
        /// </summary>
        /// <param name="num">#WAV定義番号</param>
        /// <returns>numが"01"から"ZZ"の範囲ならtrue</returns>
        private static bool IsInRange(int num)
        {
            if (num < 1)
            {
                return false;
            }

            if (RadixConvert.ZZToInt("ZZ") < num)
            {
                return false;
            }

            return true;
        }
    }
}
