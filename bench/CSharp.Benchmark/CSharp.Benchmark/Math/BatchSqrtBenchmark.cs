using BenchmarkDotNet.Attributes;
using CSharp.Math;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Benchmark.Math;

public class BatchSqrtBenchmark : BenchmarkBase<BatchSqrtBenchmark>
{
    public static IEnumerable<float[]> FloatSource()
    {
        yield return CreateRandomArray(3);
        yield return CreateRandomArray(4);
        yield return CreateRandomArray(5);
        yield return CreateRandomArray(7);
        yield return CreateRandomArray(86);
        yield return CreateRandomArray(87);
        yield return CreateRandomArray(88);
        yield return CreateRandomArray(89);
        yield return CreateRandomArray(10007);

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
    public float[] ComputeBatchFastInverseSquareRoot(float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            var x = array[i];
            array[i] = Sqrt.ComputeScalarFastInverseSquareRoot(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchFastInverseSquareRootUnsafe(float[] array)
    {
        return Sqrt.ComputeBatchFastInverseSquareRootUnsafe(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchSseInverseSquareRoot(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithSse(ref start, ref offset, length);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchAvxInverseSquareRoot(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithAvx(ref start, ref offset, length);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSquareRootWithAvxSseHybrid(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithAvx(ref start, ref offset, length);
        }

        if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithSse(ref start, ref offset, length);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSquareRootWithAvxFallbackSse(float[] array)
    {
        return Sqrt.ComputeBatchInverseSquareRootWithHardwareAcceleration(array);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSqrtRootWithSseDivide(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var three = Vector128.Create(3f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var value = Sse.Divide(Vector128<float>.One, Sse.Sqrt(vx));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sse.DivideScalar(Vector128<float>.One, Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x))).ToScalar();
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSqrtRootWithAvxDivide(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var three = Vector256.Create(3f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var value = Avx.Divide(Vector256<float>.One, Avx.Sqrt(vx));

                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSqrtRootWithAvxSseHybridDivide(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var three = Vector256.Create(3f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var value = Avx.Divide(Vector256<float>.One, Avx.Sqrt(vx));

                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            var three = Vector128.Create(3f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var value = Sse.Divide(Vector128<float>.One, Sse.Sqrt(vx));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float[] ComputeBatchInverseSqrtRootWithAvxFallbackSseDivide(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var three = Vector256.Create(3f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var value = Avx.Divide(Vector256<float>.One, Avx.Sqrt(vx));

                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }
        else if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var three = Vector128.Create(3f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var value = Sse.Divide(Vector128<float>.One, Sse.Sqrt(vx));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = Sqrt.ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }
}
