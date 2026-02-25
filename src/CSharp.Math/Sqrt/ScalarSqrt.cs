using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math.Sqrt;

[Display]
public class ScalarSqrt
{
    public static void Execute()
    {
        var x = 1311.22f;
        var v1 = FastInverseSquareRoot(x);
        var sse_2 = Sse_2(x);
        var sse_3 = Sse_3(x);
        var sse_4 = Sse_4(x);
        var avx_1 = Avx_1(x);
        var avx_2 = Avx_2(x);
        var avx_3 = Avx_3(x);
    }

    /// <summary>
    /// 快速倒数平方根
    /// <code/>
    /// Fast 1/sqrt(x) using Quake III method
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastInverseSquareRoot(float x)
    {
        int i = BitConverter.SingleToInt32Bits(x);
        i = 0x5f3759df - (i >> 1);
        float y = BitConverter.Int32BitsToSingle(i);

        // 牛顿迭代（Newton–Raphson）公式
        return y * (1.5f - 0.5f * x * y * y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sse_1(float x)
    {
        if (Sse.IsSupported)
        {
            var vx = Vector128.CreateScalarUnsafe(x);
            var rcp = Sse.ReciprocalSqrtScalar(vx);
            var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
            var half = Vector128.CreateScalarUnsafe(0.5f);
            // y * (1.5f - 0.5f * x * y * y)
            var value = Sse.Multiply(rcp, Sse.Subtract(onePointFive, Sse.Multiply(half, Sse.Multiply(vx, Sse.Multiply(rcp, rcp)))));
            return value.ToScalar();
        }

        return FastInverseSquareRoot(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sse_2(float x)
    {
        if (Sse.IsSupported)
        {
            var vx = Vector128.CreateScalarUnsafe(x);
            var rcp = Sse.ReciprocalSqrtScalar(vx);
            var three = Vector128.CreateScalarUnsafe(3f);
            var half = Vector128.CreateScalarUnsafe(0.5f);
            // 0.5f * y * (3f - x * y * y)
            var value = Sse.Multiply(half, Sse.Multiply(rcp, Sse.Subtract(three, Sse.Multiply(vx, Sse.Multiply(rcp, rcp)))));
            return value.ToScalar();
        }

        return FastInverseSquareRoot(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sse_3(float x)
    {
        if (Sse.IsSupported)
        {
            return 1 / Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x)).ToScalar();
        }

        return FastInverseSquareRoot(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sse_4(float x)
    {
        if (Sse.IsSupported)
        {
            // _mm_div_ss (Scalar Single)
            return Sse.DivideScalar(Vector128<float>.One, Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x))).ToScalar();
        }

        return FastInverseSquareRoot(x);
    }

    // 不推荐使用，精度差
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static float Sse_5(float x)
    //{
    //    if (Sse.IsSupported)
    //    {
    //        return Sse.ReciprocalScalar(Sse.SqrtScalar(Vector128.CreateScalarUnsafe(x))).ToScalar();
    //    }

    //    return FastInverseSquareRoot(x);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Avx_1(float x)
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

        return FastInverseSquareRoot(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Avx_2(float x)
    {
        if (Avx.IsSupported)
        {
            var vx = Vector256.CreateScalarUnsafe(x);
            var rcp = Avx.ReciprocalSqrt(vx);
            var three = Vector256.CreateScalarUnsafe(3f);
            var half = Vector256.CreateScalarUnsafe(0.5f);
            // 0.5f * y * (3f - x * y * y)
            var value = Avx.Multiply(half, Avx.Multiply(rcp, Avx.Subtract(three, Avx.Multiply(vx, Avx.Multiply(rcp, rcp)))));
            return value.ToScalar();
        }

        return FastInverseSquareRoot(x);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Avx_3(float x)
    {
        if (Avx.IsSupported)
        {
            return 1 / Avx.Sqrt(Vector256.CreateScalarUnsafe(x)).ToScalar();
        }

        return FastInverseSquareRoot(x);
    }
}
