using BenchmarkDotNet.Attributes;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Benchmark.Math;

public class SqrtScalarBenchmark : BenchmarkBase<SqrtScalarBenchmark>
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
    public float InverseSqrtMagic(float x)
    {
        int i = BitConverter.SingleToInt32Bits(x);
        i = 0x5f3759df - (i >> 1);
        float y = BitConverter.Int32BitsToSingle(i);
        return y * (1.5f - 0.5f * x * y * y);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float InverseSqrt(float x)
    {
        if (Sse.IsSupported)
        {
            var vx = Vector128.CreateScalarUnsafe(x);
            var approx = Sse.ReciprocalSqrtScalar(vx);
            var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
            var half = Vector128.CreateScalarUnsafe(0.5f);

            var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
            var error = Sse.SubtractScalar(onePointFive, Sse.MultiplyScalar(half, xTimesApproxSq));
            return Sse.MultiplyScalar(approx, error).ToScalar();
        }

        throw new InvalidOperationException("Sse not supported!");
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float ComputeScalarSseInverseSquareRootDirect(float x)
    {
        if (Sse.IsSupported)
        {
            return 1 / Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x)).ToScalar();
        }

        throw new InvalidOperationException("Sse not supported!");
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

        throw new InvalidOperationException("Sse not supported!");
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

        throw new InvalidOperationException("Avx not supported!");
    }
}
