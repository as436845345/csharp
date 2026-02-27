using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Power
    {
        internal static void Execute()
        {
            //Console.WriteLine("[Power]");

            //const float X = 3.3f;
            //const float Y = 50f;

            //var a = Y * MathF.Log2(X);
            //var b = Y * Log2.Approximated(X);
            //var c = 1 - a / b;

            //Console.WriteLine(a);
            //Console.WriteLine(b);
            //Console.WriteLine(c);

            //Console.WriteLine(MathF.Pow(X, Y));
            //Console.WriteLine(Exponential.Exp2(b));
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
            return Exponential.Exp(y * Log2.Approximated(x) * LN2);

            // x^y = 2^(y·log2(x))
            //return Exponential.Exp2(y * Log2.Approximated(x));
        }
    }
}