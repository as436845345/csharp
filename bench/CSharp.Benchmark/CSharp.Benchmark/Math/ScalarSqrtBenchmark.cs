using BenchmarkDotNet.Attributes;
using CSharp.Math;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

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
        return Sqrt.ComputeScalarFastInverseSquareRoot(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarInverseSquareRootWithSse(float x)
    {
        return Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootDirect(float x)
    {
        if (Sse.IsSupported)
        {
            return 1 / Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x)).ToScalar();
        }

        return Sqrt.ComputeScalarFastInverseSquareRoot(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootDivide(float x)
    {
        if (Sse.IsSupported)
        {
            // _mm_div_ss (Scalar Single)
            return Sse.DivideScalar(Vector128<float>.One, Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x))).ToScalar();
        }

        return Sqrt.ComputeScalarFastInverseSquareRoot(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarAvxInverseSquareRootNewton(float x)
    {
        if (Avx.IsSupported)
        {
            var vx = Vector256.CreateScalarUnsafe(x);
            var rcp = Avx.ReciprocalSqrt(vx);
            var onePointFive = Vector256.CreateScalarUnsafe(1.5f);
            var half = Vector256.CreateScalarUnsafe(0.5f);
            // y * (1.5f - 0.5f * x * y * y)
            var value = Avx.Multiply(rcp, Avx.Subtract(onePointFive, Avx.Multiply(half, Avx.Multiply(vx, Avx.Multiply(rcp, rcp)))));
            return value.ToScalar();
        }

        return Sqrt.ComputeScalarFastInverseSquareRoot(x);
    }
}
