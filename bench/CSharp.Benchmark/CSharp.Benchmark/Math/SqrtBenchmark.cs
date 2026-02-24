using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

public class SqrtBenchmark : BenchmarkBase<SqrtBenchmark>
{
    public static IEnumerable<float> FloatSource()
    {
        yield return 2f;
        yield return 2.5f;
        yield return 3f;
        yield return 4f;
        yield return 5f;
        yield return 6f;
        yield return 7f;
        yield return 10f;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(FloatSource))]
    public float MathSqrt(float x)
    {
        return (float)System.Math.Sqrt(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float FastInverseSquareRoot(float x)
    {
        return Sqrt.FastInverseSquareRoot(x);
    }
}
