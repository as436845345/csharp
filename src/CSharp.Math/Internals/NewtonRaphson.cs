using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

internal static class NewtonRaphson
{
    /// <summary>
    /// 对 1/√x 的初始近似值执行一次牛顿 - 拉夫逊迭代优化
    /// </summary>
    /// <param name="x">目标值（x > 0）</param>
    /// <param name="initialApprox">初始近似值（建议由 <see cref="Sse.ReciprocalSqrtScalar(Vector128{float})"/> 或魔数法提供）</param>
    /// <returns>优化后的近似值（精度 ~23 位）</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RefineInverseSqrt(float x, float initialApprox)
    {
        return initialApprox * (1.5f - 0.5f * x * initialApprox * initialApprox);
    }

    /// <summary>
    /// 标量：使用 SSE + 牛顿迭代计算 1/√x
    /// </summary>
    /// <param name="x">输入值（x > 0）</param>
    /// <returns>1/√x 的高精度近似值（精度 ~23 位）</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseSqrtSse(float x)
    {
        var vx = Vector128.CreateScalarUnsafe(x);
        var approx = Sse.ReciprocalSqrtScalar(vx);
        var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
        var half = Vector128.CreateScalarUnsafe(0.5f);
        var xTimesApproxSq = Sse.MultiplyScalar(vx, Sse.MultiplyScalar(approx, approx));
        var error = Sse.SubtractScalar(onePointFive, Sse.MultiplyScalar(half, xTimesApproxSq));
        return Sse.MultiplyScalar(approx, error).ToScalar();
    }

    /// <summary>
    /// 批量：SSE 128-bit 向量化计算 1/√x
    /// </summary>
    /// <param name="start">数组起始引用</param>
    /// <param name="offset">当前偏移量</param>
    /// <param name="length">数组总长度</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InverseSqrtBatchSse(ref float start, ref int offset, int length)
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

    /// <summary>
    /// 批量：AVX 256-bit 向量化计算 1/√x
    /// </summary>
    /// <param name="start">数组起始引用</param>
    /// <param name="offset">当前偏移量</param>
    /// <param name="length">数组总长度</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InverseSqrtBatchAvx(ref float start, ref int offset, int length)
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
}
