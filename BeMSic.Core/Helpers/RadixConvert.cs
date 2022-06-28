namespace BeMSic.Core.Helpers
{
    /// <summary>
    /// 基数変換
    /// </summary>
    public static class RadixConvert
    {
        private const int ZZRadix = 36;

        /// <summary>
        /// Convert integer to 2-digit 36-ary number
        /// </summary>
        /// <param name="dec">integer</param>
        /// <returns>2-digit 36-ary number</returns>
        public static string IntToZZ(int dec)
        {
            return new string(new char[]
            {
                IntToZ(dec / ZZRadix),
                IntToZ(dec % ZZRadix),
            });
        }

        /// <summary>
        /// Convert 2-digit 36-ary number(string) to integer
        /// </summary>
        /// <param name="zz">2-digit 36-ary number</param>
        /// <returns>integer</returns>
        public static int ZZToInt(string zz)
        {
            if (zz.Length != 2)
            {
                throw new ArgumentOutOfRangeException();
            }

            int result = (ZToInt(Convert.ToChar(zz[0])) * ZZRadix) + ZToInt(Convert.ToChar(zz[1]));
            return result;
        }

        /// <summary>
        /// Convert integer to 1-digit 36-ary number
        /// </summary>
        /// <param name="value">integer</param>
        /// <returns>1-digit 36-ary number</returns>
        private static char IntToZ(int value)
        {
            if ((value >= 0) && (value <= 9))
            {
                return (char)(value + '0');
            }

            if ((value >= 10) && (value < ZZRadix))
            {
                return (char)(value - 10 + 'A');
            }

            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Convert 1-digit 36-ary number(string) to integer
        /// </summary>
        /// <param name="c">1-digit 36-ary number</param>
        /// <returns>integer</returns>
        private static int ZToInt(char c)
        {
            // 0-9
            if ((c >= '0') && (c <= '9'))
            {
                return c - '0';
            }

            // A-Z
            if ((c >= 'A') && (c <= 'Z'))
            {
                return c - 'A' + 10;
            }

            return 0;
        }
    }
}
