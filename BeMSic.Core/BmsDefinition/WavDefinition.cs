using BeMSic.Core.Helpers;

namespace BeMSic.Core.BmsDefinition
{
    /// <summary>
    /// #WAV定義
    /// </summary>
    public class WavDefinition : IEquatable<WavDefinition>, IComparable<WavDefinition>
    {
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
        /// #WAV(整数値)
        /// </summary>
        public int Num { get; }

        /// <summary>
        /// #WAV(ZZ文字列)
        /// </summary>
        public string ZZ { get; }

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
        /// get hash code
        /// </summary>
        /// <returns>hash code</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Num, ZZ);
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="other">他インスタンス</param>
        /// <returns>比較値</returns>
        public int CompareTo(WavDefinition? other)
        {
            if (other == null)
            {
                return int.MinValue;
            }

            return Num - other.Num;
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
