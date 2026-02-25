using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

public class VectorSqrtBenchmark : BenchmarkBase<VectorSqrtBenchmark>
{
    public static IEnumerable<float[]> FloatSource()
    {
        yield return CreateRandomArray(83);
        yield return CreateRandomArray(84);
        yield return CreateRandomArray(86);

        static float[] CreateRandomArray(int length)
        {
            var array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
            }
            return array;
        }
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeVectorFastInverseSquareRoot(float[] array)
    {
        return VectorSqrt.ComputeVectorFastInverseSquareRoot(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeVectorFastInverseSquareRootUnsafe(float[] array)
    {
        return VectorSqrt.ComputeVectorFastInverseSquareRootUnsafe(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeSseInverseSqrtNewton(float[] array)
    {
        return VectorSqrt.ComputeSseInverseSqrtNewton(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeSseInverseSqrtDivide(float[] array)
    {
        return VectorSqrt.ComputeSseInverseSqrtDivide(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxInverseSqrtNewton(float[] array)
    {
        return VectorSqrt.ComputeAvxInverseSqrtNewton(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxSseHybridInverseSqrtNewton(float[] array)
    {
        return VectorSqrt.ComputeAvxSseHybridInverseSqrtNewton(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxFallbackSseInverseSqrtNewton(float[] array)
    {
        return VectorSqrt.ComputeAvxFallbackSseInverseSqrtNewton(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxInverseSqrtDivide(float[] array)
    {
        return VectorSqrt.ComputeAvxInverseSqrtDivide(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxSseHybridInverseSqrtDivide(float[] array)
    {
        return VectorSqrt.ComputeAvxSseHybridInverseSqrtDivide(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeAvxFallbackSseInverseSqrtDivide(float[] array)
    {
        return VectorSqrt.ComputeAvxFallbackSseInverseSqrtDivide(array);
    }
}
