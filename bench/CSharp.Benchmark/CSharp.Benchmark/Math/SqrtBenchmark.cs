using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

public class SqrtBenchmark : BenchmarkBase<SqrtBenchmark>
{
    public static IEnumerable<float> FloatSource()
    {
        yield return 2.436223E+37f;
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

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_1(float x)
    {
        return Sqrt.Sse_1(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_2(float x)
    {
        return Sqrt.Sse_2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_3(float x)
    {
        return Sqrt.Sse_3(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_4(float x)
    {
        return Sqrt.Sse_4(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_1(float x)
    {
        return Sqrt.Avx_1(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_2(float x)
    {
        return Sqrt.Avx_2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_3(float x)
    {
        return Sqrt.Avx_3(x);
    }
}
