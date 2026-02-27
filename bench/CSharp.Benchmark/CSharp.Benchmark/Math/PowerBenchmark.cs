using BenchmarkDotNet.Attributes;
using CSharp.Benchmark.Math.Internals;

namespace CSharp.Benchmark.Math;

public class PowerBenchmark : BenchmarkBase<PowerBenchmark>
{
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
}
