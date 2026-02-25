using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 基于SSE指令集（128位向量）批量计算浮点数数组的平方根倒数（1/√x）
    /// </summary>
    /// <param name="start">浮点数组的起始引用（输入输出复用同一内存区域）</param>
    /// <param name="offset">数组起始偏移量（以float为单位），方法内会自动递增</param>
    /// <param name="length">数组总长度（以float为单位）</param>
    /// <remarks>
    /// 1. 核心逻辑：牛顿-拉夫逊迭代法，公式 yₙ₊₁ = yₙ * (1.5 - 0.5 * x * yₙ²)
    /// 2. 指令集：使用SSE的ReciprocalSqrt获取初始近似值，批量处理4个float（Vector128<float>.Count = 4）
    /// 3. 内存操作：LoadUnsafe/StoreUnsafe直接操作内存，无额外拷贝开销
    /// 4. 循环逻辑：仅处理长度为4的整数倍的部分，剩余元素需单独处理
    /// 5. 输入输出：计算结果直接覆盖原数组内存，节省内存占用
    /// 6. 适用场景：高性能批量数值计算（如图形学、物理引擎的向量归一化）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ComputeBatchInverseSquareRootWithSse(ref float start, ref int offset, int length)
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
    /// 基于AVX指令集（256位向量）批量计算浮点数数组的平方根倒数（1/√x）
    /// </summary>
    /// <param name="start">浮点数组的起始引用（输入输出复用同一内存区域）</param>
    /// <param name="offset">数组起始偏移量（以float为单位），方法内会自动递增</param>
    /// <param name="length">数组总长度（以float为单位）</param>
    /// <remarks>
    /// 1. 核心逻辑：牛顿-拉夫逊迭代法，公式 yₙ₊₁ = yₙ * (1.5 - 0.5 * x * yₙ²)
    /// 2. 指令集：使用AVX的ReciprocalSqrt获取初始近似值，批量处理8个float（Vector256<float>.Count = 8）
    /// 3. 内存操作：LoadUnsafe/StoreUnsafe直接操作内存，无额外拷贝开销
    /// 4. 循环逻辑：仅处理长度为8的整数倍的部分，剩余元素需单独处理
    /// 5. 输入输出：计算结果直接覆盖原数组内存，节省内存占用
    /// 6. 性能优势：AVX（256位）比SSE（128位）吞吐量提升约一倍，需CPU支持AVX指令集
    /// 7. 适用场景：大规模批量数值计算（如深度学习、高性能科学计算）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ComputeBatchInverseSquareRootWithAvx(ref float start, ref int offset, int length)
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
