namespace BeMSic.Core.Helpers
{
    /// <summary>
    /// 計算
    /// </summary>
    public class CalcurateEx
    {
        /// <summary>
        /// aとbの最大公約数を取得
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>aとbの最大公約数</returns>
        public static ulong Gcd(ulong a, ulong b)
        {
            if (a < b)
            {
                (a, b) = (b, a);
            }

            return GcdRecursive(a, b);
        }

        /// <summary>
        /// 最大公約数計算(再帰)
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <returns>最大公約数(再帰)</returns>
        private static ulong GcdRecursive(ulong a, ulong b)
        {
            return (b == 0) ? a : GcdRecursive(b, a % b);
        }
    }
}
