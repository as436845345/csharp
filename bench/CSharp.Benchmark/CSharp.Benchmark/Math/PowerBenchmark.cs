using BenchmarkDotNet.Attributes;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

public class PowerBenchmark : BenchmarkBase<PowerBenchmark>
{
    public static IEnumerable<object[]> Numbers()
    {
        yield return [3.1f, 4];
        yield return [3.1f, 8];
        yield return [3.1f, 13];
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
}
