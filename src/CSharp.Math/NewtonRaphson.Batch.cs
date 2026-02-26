using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 批量计算 1/√x（平方根倒数），SSE 128-bit 向量化加速
    /// </summary>
    /// <param name="values">输入/输出数据（原地修改，长度需 ≥ 4）</param>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>公式：y₁ = y₀ × (1.5 - 0.5 × x × y₀²)</description></item>
    ///   <item><description>吞吐：4 元素/迭代，延迟 ~18 周期/批</description></item>
    ///   <item><description>精度：~23 位（单次牛顿迭代）</description></item>
    ///   <item><description>前置：values.Length ≥ 4 且为 4 的倍数（调用方负责对齐）</description></item>
    /// </list>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InverseSqrtBatchSse(ref float start, ref int offset, int length)
    {
        // 预创建常量向量（避免循环内重复创建，提升性能）
        var onePointFive = Vector128.Create(1.5f);
        var half = Vector128.Create(0.5f);

        do
        {
            // 从内存加载4个float到128位向量（偏移量以float为单位）
            var vx = Vector128.LoadUnsafe(ref start, (nuint)offset);
            // SSE硬件指令：批量获取4个float的平方根倒数初始近似值
            var rcp = Sse.ReciprocalSqrt(vx);
            // 计算初始近似值的平方（yₙ²）
            var powerOfVx = Vector128.Multiply(rcp, rcp);
            // 牛顿迭代计算：yₙ * (1.5 - 0.5 * x * yₙ²)
            var value = Sse.Multiply(rcp, Sse.Subtract(onePointFive, Sse.Multiply(half, Sse.Multiply(vx, powerOfVx))));

            // 将计算结果写回原内存地址（覆盖原始值）
            Vector128.StoreUnsafe(value, ref start, (nuint)offset);
        }
        // 每次处理4个元素，偏移量递增4，直到剩余元素不足4个
        while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
    }

    /// <summary>
    /// 批量计算 1/√x（平方根倒数），AVX 256-bit 向量化加速
    /// </summary>
    /// <param name="values">输入/输出数据（原地修改，长度需 ≥ 8）</param>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>公式：同 <see cref="InverseSqrtBatchSse"/></description></item>
    ///   <item><description>吞吐：8 元素/迭代，理论加速比 ~2× vs SSE</description></item>
    ///   <item><description>前置：values.Length ≥ 8 且为 8 的倍数</description></item>
    /// </list>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InverseSqrtBatchAvx(ref float start, ref int offset, int length)
    {
        // 预创建常量向量（避免循环内重复创建，提升性能）
        var onePointFive = Vector256.Create(1.5f);
        var half = Vector256.Create(0.5f);

        do
        {
            // 从内存加载8个float到256位向量（偏移量以float为单位）
            var vx = Vector256.LoadUnsafe(ref start, (nuint)offset);
            // AVX硬件指令：批量获取8个float的平方根倒数初始近似值
            var rcp = Avx.ReciprocalSqrt(vx);
            // 计算初始近似值的平方（yₙ²）
            var powerOfVx = Vector256.Multiply(rcp, rcp);
            // 牛顿迭代计算：yₙ * (1.5 - 0.5 * x * yₙ²)
            var value = Avx.Multiply(rcp, Avx.Subtract(onePointFive, Avx.Multiply(half, Avx.Multiply(vx, powerOfVx))));

            // 将计算结果写回原内存地址（覆盖原始值）
            Vector256.StoreUnsafe(value, ref start, (nuint)offset);
        }
        // 每次处理8个元素，偏移量递增8，直到剩余元素不足8个
        while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
    }
}
