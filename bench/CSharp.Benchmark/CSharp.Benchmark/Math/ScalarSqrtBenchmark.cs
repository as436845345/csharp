using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

public class ScalarSqrtBenchmark : BenchmarkBase<ScalarSqrtBenchmark>
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
        return ScalarSqrt.FastInverseSquareRoot(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_1(float x)
    {
        return ScalarSqrt.Sse_1(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_2(float x)
    {
        return ScalarSqrt.Sse_2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_3(float x)
    {
        return ScalarSqrt.Sse_3(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Sse_4(float x)
    {
        return ScalarSqrt.Sse_4(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_1(float x)
    {
        return ScalarSqrt.Avx_1(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_2(float x)
    {
        return ScalarSqrt.Avx_2(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float Avx_3(float x)
    {
        return ScalarSqrt.Avx_3(x);
    }
}
