using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Power
    {
        internal static void Execute()
        {
            Console.WriteLine("[Power]");

            const float X = 3.3f;
            const float Y = 50.55555f;

            var mfp = MathF.Pow(X, Y);
            var p2 = Pow2(X, Y);
            var c = 1 - p2 / mfp;

            Console.WriteLine(mfp);
            Console.WriteLine(p2);
            Console.WriteLine(c);

            Console.WriteLine(Pow(X, Y));
        }

        /// <summary>
        /// 高性能浮点数整数次幂运算。（直接用 <see cref="MathF.Pow(float, float)"/> 替代）
        /// </summary>
        /// <param name="x">底数 (float)</param>
        /// <param name="n">指数 (int)</param>
        /// <returns>x 的 n 次幂</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, int n)
        {
            // 快速幂
            bool navegate = n < 0;
            n = System.Math.Abs(n);

            float result = 1f;
            float power = x;

            while (n > 0)
            {
                if ((n & 1) == 1)
                    result *= power;

                power *= power;

                n >>= 1;
            }

            return navegate ? 1f / result : result;
        }

        /// <summary>
        /// Fast general pow(x, y) = exp(y * log(x)).（误差很大，用 <see cref="MathF.Pow(float, float)"/> 替代）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow(float x, float y)
        {
            // General case: exp(y * log(x))
            // log(x) = log2(x) * loge(2)
            return Exponential.Exp(y * Log2.Approximated(x) * LN2);

            // x^y = 2^(y·log2(x))
            //return Exponential.Exp2(y * Log2.Approximated(x));
        }

        /// <summary>
        /// 运算慢，用 <see cref="MathF.Pow(float, float)"/> 替代
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Pow2(float x, float y)
        {
            // x^y = x^(floor(y) * z(0.xxxxxx))
            int f = (int)MathF.Floor(y);
            float z = y - f;

            float xy = Pow(x, f);
            float xz = Exponential.Exp2(z * Log2.Approximated(x));

            return xy * xz;
        }
    }
}