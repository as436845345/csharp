using System.Runtime.CompilerServices;

namespace CSharp.Benchmark.Math.Internals;

internal static class Power
{
    /// <summary>
    /// 高性能浮点数整数次幂运算。
    /// 对小指数 (-4 ~ 8) 使用展开优化，大指数使用快速幂算法。
    /// </summary>
    /// <param name="x">底数 (float)</param>
    /// <param name="n">指数 (int)</param>
    /// <returns>x 的 n 次幂</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Pow(float x, int n)
    {
        switch (n)
        {
            case 0: return 1f;
            case 1: return x;
            case 2: return x * x;
            case 3: return x * x * x;
            case 4: { float x2 = x * x; return x2 * x2; }
            case 5: { float x2 = x * x; return x2 * x2 * x; }
            case 6: { float x2 = x * x; return x2 * x2 * x2; }
            case 7: { float x2 = x * x; float x4 = x2 * x2; return x4 * x2 * x; }
            case 8: { float x2 = x * x; float x4 = x2 * x2; return x4 * x4; }
            case 9: { float x2 = x * x; float x4 = x2 * x2; return x4 * x4 * x; }
            case 10: { float x2 = x * x; float x4 = x2 * x2; return x4 * x4 * x2; }
            case -1: return 1f / x;
            case -2: { float x2 = x * x; return 1f / x2; }
            case -3: { float x3 = x * x * x; return 1f / x3; }
            case -4: { float x2 = x * x; return 1f / (x2 * x2); }
        }

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
}
