using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class TrigBenchmark : BenchmarkBase<TrigBenchmark>
{
    public static IEnumerable<float> TrigNumbers()
    {
        yield return Trig.PI / 12;
        yield return Trig.PI / 6;
        yield return Trig.PI / 4;
        yield return Trig.PI / 3;
    }

    #region Sin

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Sin")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float MathFSin(float x)
    {
        return MathF.Sin(x);
    }

    [Benchmark]
    [BenchmarkCategory("Sin")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float Sin(float x)
    {
        return Trig.Sin(x);
    }

    #endregion

    #region Cos

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Cos")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float MathFCos(float x)
    {
        return MathF.Cos(x);
    }

    [Benchmark]
    [BenchmarkCategory("Cos")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float Cos(float x)
    {
        return Trig.Cos(x);
    }

    #endregion

    #region Tan

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Tan")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float MathFTan(float x)
    {
        return MathF.Tan(x);
    }

    [Benchmark]
    [BenchmarkCategory("Tan")]
    [ArgumentsSource(nameof(TrigNumbers))]
    public float Tan(float x)
    {
        return Trig.Tan(x);
    }

    #endregion
}
