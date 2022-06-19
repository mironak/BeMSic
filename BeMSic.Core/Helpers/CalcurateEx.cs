namespace BeMSic.Core.Helpers
{
    public class CalcurateEx
    {
        public static ulong Gcd(ulong a, ulong b)
        {
            if(a < b)
            {
                (a, b) = (b, a);
            }
            return GcdRecursive(a, b);
        }

        private static ulong GcdRecursive(ulong a, ulong b)
        {
            return (b == 0) ? a : GcdRecursive(b, a % b);
        }
    }
}
