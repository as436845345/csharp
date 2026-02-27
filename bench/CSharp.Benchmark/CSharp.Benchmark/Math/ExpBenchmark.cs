using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class ExpBenchmark : BenchmarkBase<ExpBenchmark>
{
    #region Exp

    public static IEnumerable<float> ExpNumbers()
    {
        yield return 7.66f;
        yield return 13;
        yield return 75.82937f;
        yield return 131;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Exp")]
    [ArgumentsSource(nameof(ExpNumbers))]
    public float MathFExp(float x)
    {
        return MathF.Exp(x);
    }

    [Benchmark]
    [BenchmarkCategory("Exp")]
    [ArgumentsSource(nameof(ExpNumbers))]
    public float Exp(float x)
    {
        return Exponential.Exp(x);
    }

    [Benchmark]
    [BenchmarkCategory("Exp")]
    [ArgumentsSource(nameof(ExpNumbers))]
    public float ExpApprox(float x)
    {
        return Exponential.ExpApprox(x);
    }

    #endregion

    #region Exp2

    public static IEnumerable<float> Exp2Numbers()
    {
        yield return 0;
        yield return 1;
        yield return 2;
        yield return 3;
        yield return 4;
        yield return 5;
        yield return 6;
        yield return 7;
        yield return 8;
        yield return 9;
        yield return 10;
        yield return 13;
        yield return 77;
        yield return 127;
        yield return -1;
        yield return -30;
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Exp2")]
    [ArgumentsSource(nameof(Exp2Numbers))]
    public float MathFExp2(float x)
    {
        return MathF.Pow(2, x);
    }

    [Benchmark]
    [BenchmarkCategory("Exp2")]
    [ArgumentsSource(nameof(Exp2Numbers))]
    public float Exp2(float x)
    {
        return Exponential.Exp2(x);
    }

    [Benchmark]
    [BenchmarkCategory("Exp2")]
    [ArgumentsSource(nameof(Exp2Numbers))]
    public float Exp2Variant(float x)
    {
        switch (x)
        {
            case 0: return 1f;
            case 1f: return 2f;
            case 2f: return 4f;
            case 3f: return 8f;
            case 4f: return 16f;
            case 5f: return 32f;
            case 6f: return 64f;
            case 7f: return 128f;
            case 8f: return 256f;
            case 9f: return 512f;
            case 10f: return 1024f;
            case -1f: return 0.5f;
        }

        x = System.Math.Clamp(x, -126f, 128f);

        int n = (int)MathF.Floor(x);
        float f = x - n;

        // 2^f 拟合
        float p = 1f + f * (0.6931472f + f * (0.2402265f + f * 0.0558012f));

        // 位移构造 2^n
        int bits = (n + 127) << 23;
        return BitConverter.Int32BitsToSingle(bits) * p;
    }

    #endregion
}
