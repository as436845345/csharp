using BenchmarkDotNet.Attributes;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

public class PowerBenchmark : BenchmarkBase<PowerBenchmark>
{
    #region Power

    public static IEnumerable<object[]> Numbers()
    {
        yield return [2f, 7];
        yield return [2f, 9];
        yield return [2f, 10];
        yield return [3.1f, 9];
        yield return [3.1f, 10];
        yield return [3.1f, 11];
        yield return [3.1f, 31];
        yield return [3.1f, 77];
        yield return [5f, 9];
        yield return [5f, 10];
        yield return [5f, 11];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float MathPow(float x, float n)
    {
        return (float)System.Math.Pow(x, n);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float MathFPow(float x, float n)
    {
        return MathF.Pow(x, n);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float PowerPow(float x, int n)
    {
        return Power.Pow(x, n);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float PowerPow2(float x, int n)
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

    #endregion
}
