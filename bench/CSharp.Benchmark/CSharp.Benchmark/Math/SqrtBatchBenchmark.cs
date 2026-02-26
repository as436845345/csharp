using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using CSharp.Math;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Benchmark.Math;

[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[HardwareCounters(
        HardwareCounter.BranchMispredictions,
        HardwareCounter.BranchInstructions)]
public class SqrtBatchBenchmark : BenchmarkBase<SqrtBatchBenchmark>
{
    public static IEnumerable<float[]> FloatSource()
    {
        //yield return CreateRandomArray(3);
        //yield return CreateRandomArray(4);
        //yield return CreateRandomArray(5);
        //yield return CreateRandomArray(7);
        //yield return CreateRandomArray(86);
        //yield return CreateRandomArray(87);
        //yield return CreateRandomArray(88);
        //yield return CreateRandomArray(89);
        //yield return CreateRandomArray(10007);

        //yield return CreateRandomArray(113);
        //yield return CreateRandomArray(333);
        //yield return CreateRandomArray(603);
        //yield return CreateRandomArray(858);
        //yield return CreateRandomArray(1035);
        //yield return CreateRandomArray(2620);
        //yield return CreateRandomArray(3361);
        //yield return CreateRandomArray(4215);
        yield return CreateRandomArray(5347);

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

    #region Magic

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Magic")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchFastInverseSquareRoot(float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            ref var x = ref array[i];
            x = HighPerfMath.Sqrt.InverseSqrtMagic(x);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Magic")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchFastInverseSquareRootUnsafe(float[] array)
    {
        HighPerfMath.Sqrt.InverseSqrtBatchMagic(array);
    }

    #endregion

    #region Divide

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Divide")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSqrtRootWithSseDivide(float[] array)
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
            ref var x = ref Unsafe.Add(ref start, offset);
            x = Sse.DivideScalar(Vector128<float>.One, Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x))).ToScalar();
        }
    }

    [Benchmark]
    [BenchmarkCategory("Divide")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSqrtRootWithAvxDivide(float[] array)
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
            ref var x = ref Unsafe.Add(ref start, offset);
            x = HighPerfMath.Sqrt.InverseSqrt(x);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Divide")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSqrtRootWithAvxSseHybridDivide(float[] array)
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
            ref var x = ref Unsafe.Add(ref start, offset);
            x = HighPerfMath.Sqrt.InverseSqrt(x);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Divide")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSqrtRootWithAvxFallbackSseDivide(float[] array)
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
            ref var x = ref Unsafe.Add(ref start, offset);
            x = HighPerfMath.Sqrt.InverseSqrt(x);
        }
    }

    #endregion

    #region Simd

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchSseInverseSquareRoot(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var onePointFive = Vector128.Create(1.5f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(rcp, Sse.Subtract(onePointFive, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));
                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);

                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(onePointFive, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchAvxInverseSquareRoot(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var onePointFive = Vector256.Create(1.5f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(rcp, Avx.Subtract(onePointFive, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));
                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);

                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(onePointFive, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSquareRootWithAvxSseHybrid(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var onePointFive = Vector256.Create(1.5f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(rcp, Avx.Subtract(onePointFive, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));
                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            var onePointFive = Vector128.Create(1.5f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(rcp, Sse.Subtract(onePointFive, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));
                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var threeHalf = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);
                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(threeHalf, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSquareRootWithAvxSseHybrid2(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported)
        {
            var threeHalf = Vector256.Create(1.5f);
            var half = Vector256.Create(0.5f);

            for (; offset <= length - Vector256<float>.Count; offset += Vector256<float>.Count)
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(rcp, Avx.Subtract(threeHalf, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));
                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            }
        }

        if (Sse.IsSupported)
        {
            var threeHalf = Vector128.Create(1.5f);
            var half = Vector128.Create(0.5f);

            for (; offset <= length - Vector128<float>.Count; offset += Vector128<float>.Count)
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(rcp, Sse.Subtract(threeHalf, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));
                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            }
        }

        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var threeHalf = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);
                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(threeHalf, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSquareRootWithAvxSseHybrid3(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var threeHalf = Vector256.Create(1.5f);
            var half = Vector256.Create(0.5f);

            for (; offset <= length - Vector256<float>.Count; offset += Vector256<float>.Count)
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(rcp, Avx.Subtract(threeHalf, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));
                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            }
        }

        if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            var threeHalf = Vector128.Create(1.5f);
            var half = Vector128.Create(0.5f);

            for (; offset <= length - Vector128<float>.Count; offset += Vector128<float>.Count)
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(rcp, Sse.Subtract(threeHalf, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));
                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            }
        }

        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var threeHalf = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);
                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(threeHalf, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Simd")]
    [ArgumentsSource(nameof(FloatSource))]
    public void ComputeBatchInverseSquareRootWithAvxFallbackSse(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        // 优先使用AVX（256位）批量计算
        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var threeHalf = Vector256.Create(1.5f);
            var half = Vector256.Create(0.5f);

            do
            {
                var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(rcp, Avx.Subtract(threeHalf, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));
                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }
        // 降级使用SSE（128位）批量计算
        else if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var threeHalf = Vector128.Create(1.5f);
            var half = Vector128.Create(0.5f);

            do
            {
                var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(rcp, Sse.Subtract(threeHalf, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));
                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            }
            while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        // 处理剩余不足256/128位的元素（标量硬件加速版）
        for (; offset < length; offset++)
        {
            ref var x = ref Unsafe.Add(ref start, offset);

            if (Sse.IsSupported)
            {
                var vx = Vector128.CreateScalarUnsafe(x);
                var approx = Sse.ReciprocalSqrtScalar(vx);
                var threeHalf = Vector128.CreateScalarUnsafe(1.5f);
                var half = Vector128.CreateScalarUnsafe(0.5f);
                var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
                var error = Sse.SubtractScalar(threeHalf, Sse.MultiplyScalar(half, xTimesApproxSq));
                x = Sse.MultiplyScalar(approx, error).ToScalar();
            }
            else
            {
                int i = BitConverter.SingleToInt32Bits(x);
                i = 0x5f3759df - (i >> 1);
                float y = BitConverter.Int32BitsToSingle(i);
                x = y * (1.5f - 0.5f * x * y * y);
            }
        }
    }

    #endregion
}
