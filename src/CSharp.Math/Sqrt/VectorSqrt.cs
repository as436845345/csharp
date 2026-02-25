using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math.Sqrt;

[Display]
public class VectorSqrt
{
    public static void Execute()
    {
        var array = new float[37];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
        }

        var f1_v = ComputeVectorFastInverseSquareRoot(array.AsSpan().ToArray());
        var f2_v = ComputeVectorFastInverseSquareRootUnsafe(array.AsSpan().ToArray());

        var sse1_v = ComputeSseInverseSqrtNewton(array.AsSpan().ToArray());
        var sse_divide_1_v = ComputeSseInverseSqrtDivide(array.AsSpan().ToArray());

        var avx1_v = ComputeAvxInverseSqrtNewton(array.AsSpan().ToArray());
        var avx2_v = ComputeAvxSseHybridInverseSqrtNewton(array.AsSpan().ToArray());
        var avx3_v = ComputeAvxFallbackSseInverseSqrtNewton(array.AsSpan().ToArray());

        var avx_divide_1_v = ComputeAvxInverseSqrtDivide(array.AsSpan().ToArray());
        var avx_divide_2_v = ComputeAvxSseHybridInverseSqrtDivide(array.AsSpan().ToArray());
        var avx_divide_3_v = ComputeAvxFallbackSseInverseSqrtDivide(array.AsSpan().ToArray());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeVectorFastInverseSquareRoot(float[] array)
    {
        // 含边界检测

        for (int i = 0; i < array.Length; i++)
        {
            var x = array[i];
            array[i] = ScalarSqrt.FastInverseSquareRoot(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeVectorFastInverseSquareRootUnsafe(float[] array)
    {
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        for (int i = 0; i < array.Length; i++)
        {
            var x = Unsafe.Add(ref start, i);
            Unsafe.Add(ref start, i) = ScalarSqrt.FastInverseSquareRoot(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeSseInverseSqrtNewton(float[] array)
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
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(half, Sse.Multiply(rcp, Sse.Subtract(three, Sse.Multiply(vx, powerOfVx))));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeSseInverseSqrtDivide(float[] array)
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
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_4(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxInverseSqrtNewton(float[] array)
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
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(half, Avx.Multiply(rcp, Avx.Subtract(three, Avx.Multiply(vx, powerOfVx))));

                Vector256.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxSseHybridInverseSqrtNewton(float[] array)
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
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(half, Avx.Multiply(rcp, Avx.Subtract(three, Avx.Multiply(vx, powerOfVx))));

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
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(half, Sse.Multiply(rcp, Sse.Subtract(three, Sse.Multiply(vx, powerOfVx))));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxFallbackSseInverseSqrtNewton(float[] array)
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
                var rcp = Avx.ReciprocalSqrt(vx);
                var powerOfVx = Vector256.Multiply(rcp, rcp);
                var value = Avx.Multiply(half, Avx.Multiply(rcp, Avx.Subtract(three, Avx.Multiply(vx, powerOfVx))));

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
                var rcp = Sse.ReciprocalSqrt(vx);
                var powerOfVx = Vector128.Multiply(rcp, rcp);
                var value = Sse.Multiply(half, Sse.Multiply(rcp, Sse.Subtract(three, Sse.Multiply(vx, powerOfVx))));

                Vector128.StoreUnsafe(value, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxInverseSqrtDivide(float[] array)
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
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxSseHybridInverseSqrtDivide(float[] array)
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
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeAvxFallbackSseInverseSqrtDivide(float[] array)
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
            Unsafe.Add(ref start, offset) = ScalarSqrt.Sse_2(x);
        }

        return array;
    }
}
