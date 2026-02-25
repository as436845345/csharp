using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

public class ScalarSqrtBenchmark : BenchmarkBase<ScalarSqrtBenchmark>
{
    public static IEnumerable<float> FloatSource()
    {
        yield return Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
        yield return Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
        yield return Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(FloatSource))]
    public float MathSqrt(float x)
    {
        return 1 / (float)System.Math.Sqrt(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float MathReciprocalSqrtEstimate(float x)
    {
        return (float)System.Math.ReciprocalSqrtEstimate(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarFastInverseSquareRoot(float x)
    {
        return ScalarSqrt.ComputeScalarFastInverseSquareRoot(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootNewtonQuake(float x)
    {
        return ScalarSqrt.ComputeScalarSseInverseSquareRootNewtonQuake(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootNewtonOptimized(float x)
    {
        return ScalarSqrt.ComputeScalarSseInverseSquareRootNewtonOptimized(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootDirect(float x)
    {
        return ScalarSqrt.ComputeScalarSseInverseSquareRootDirect(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootDivide(float x)
    {
        return ScalarSqrt.ComputeScalarSseInverseSquareRootDivide(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarAvxInverseSquareRootNewtonQuake(float x)
    {
        return ScalarSqrt.ComputeScalarAvxInverseSquareRootNewtonQuake(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarAvxInverseSquareRootNewtonOptimized(float x)
    {
        return ScalarSqrt.ComputeScalarAvxInverseSquareRootNewtonOptimized(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarAvxInverseSquareRootDirect(float x)
    {
        return ScalarSqrt.ComputeScalarAvxInverseSquareRootDirect(x);
    }
}
