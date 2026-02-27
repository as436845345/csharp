using BenchmarkDotNet.Attributes;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

public class PowerBenchmark : BenchmarkBase<PowerBenchmark>
{
    public static IEnumerable<object[]> Numbers()
    {
        yield return [3.1f, 7];
        yield return [3.1f, 9];
        yield return [3.1f, 10.55555555f];
        yield return [3.1f, 31.1232151361f];
        yield return [3.1f, 76.9278684f];
        yield return [11f, 9.26631f];
        yield return [11f, 10.6321f];
        yield return [13f, 3.3f];
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float MathFPow(float x, float y)
    {
        return MathF.Pow(x, y);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Numbers))]
    public float PowerPow(float x, float y)
    {
        return Power.Pow(x, y);
    }
}
