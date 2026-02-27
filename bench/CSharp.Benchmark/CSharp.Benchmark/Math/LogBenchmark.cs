using BenchmarkDotNet.Attributes;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

public class LogBenchmark : BenchmarkBase<LogBenchmark>
{
    public static IEnumerable<float> Floats()
    {
        yield return 13f;
        yield return 167f;
        yield return 312f;
        yield return 677f;
    }

    #region Log2

    [Benchmark]
    [ArgumentsSource(nameof(Floats))]
    public float MathLog2(float x)
    {
        return (float)System.Math.Log2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Floats))]
    public float MathFLog2(float x)
    {
        return MathF.Log2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Floats))]
    public float Log2Approximated(float x)
    {
        return Log.Log2.Approximated(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Floats))]
    public float Log2Estimated(float x)
    {
        return Log.Log2.Estimated(x);
    }

    #endregion
}
