using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// #WAV定義
    /// </summary>
    public class WavDefinition : IEquatable<WavDefinition>, IComparable<WavDefinition>
    {
        public readonly int Num;
        public readonly string ZZ;

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
            ZZ = RadixConvert.IntToZZ(num);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="zz">#WAVインデックス(文字列ZZ)</param>
        public WavDefinition(string zz)
        {
            var num = RadixConvert.ZZToInt(zz);

            if (!IsInRange(num))
            {
                throw new ArgumentOutOfRangeException(nameof(zz));
            }

            Num = num;
            ZZ = zz;
        }

        /// <summary>
        /// 等価演算
        /// </summary>
        /// <param name="other">other</param>
        /// <returns>等価ならtrue</returns>
        public bool Equals(WavDefinition? other)
        {
            if (other == null)
            {
                return false;
            }

            return Num == other.Num;
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

        public int CompareTo(WavDefinition? other)
        {
            if (other == null)
            {
                return int.MinValue;
            }

            return Num - other.Num;
        }
    }
}
