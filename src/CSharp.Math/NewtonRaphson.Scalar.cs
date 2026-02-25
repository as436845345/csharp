using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 基于SSE指令集的牛顿-拉夫逊迭代法计算 1/x（倒数）
    /// </summary>
    /// <param name="x">输入值（要求 x ≠ 0）</param>
    /// <returns>1/x 的高精度近似值</returns>
    /// <remarks>
    /// 1. 牛顿迭代公式：yₙ₊₁ = yₙ * (2 - x * yₙ)
    /// 2. 纯SSE硬件指令加速，无托管代码开销，适用于高性能数值计算场景
    /// 3. 二次收敛特性，单次迭代即可将初始近似值精度提升至单精度浮点数上限
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ComputeScalarInverseWithSse(float x)
    {
        // 创建单精度浮点数的Vector128标量（避免装箱/拆箱，提升指令执行效率）
        var vectorX = Vector128.CreateScalarUnsafe(x);

        // SSE硬件指令：快速获取1/x的初始粗糙近似值
        var initialApproximation = Sse.ReciprocalScalar(vectorX);

        // 预定义常量的Vector128标量（减少重复创建，提升性能）
        var two = Vector128.CreateScalarUnsafe(2f);

        // 计算 2 - x * yₙ
        var subtractResult = Sse.SubtractScalar(two, Sse.MultiplyScalar(vectorX, initialApproximation));

        // 最终迭代计算：yₙ * (2 - x * yₙ)
        var finalResult = Sse.MultiplyScalar(initialApproximation, subtractResult);

        // 将Vector128标量转换为普通float返回（无额外性能开销）
        return finalResult.ToScalar();
    }

    /// <summary>
    /// 基于SSE指令集的牛顿-拉夫逊迭代法计算 1/√x（平方根倒数）
    /// </summary>
    /// <param name="x">输入值（要求 x > 0）</param>
    /// <returns>1/√x 的高精度近似值</returns>
    /// <remarks>
    /// 1. 先通过SSE的ReciprocalSqrtScalar获取初始粗糙近似值
    /// 2. 再通过牛顿迭代公式优化精度
    /// 3. 纯硬件指令加速，适用于高性能计算场景（如图形学、游戏引擎）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ComputeScalarInverseSquareRootWithSse(float x)
    {
        // 创建单精度浮点数的Vector128标量（避免装箱/拆箱）
        var vectorX = Vector128.CreateScalarUnsafe(x);

        // SSE硬件指令：快速获取1/√x的初始粗糙近似值
        var initialApproximation = Sse.ReciprocalSqrtScalar(vectorX);

        // 预定义常量的Vector128标量（减少重复创建）
        var onePointFive = Vector128.CreateScalarUnsafe(1.5f);
        var half = Vector128.CreateScalarUnsafe(0.5f);

        // 计算 yₙ²（初始近似值的平方）
        var squaredApproximation = Sse.MultiplyScalar(initialApproximation, initialApproximation);

        // 计算 0.5 * x * yₙ²
        var tempValue = Sse.MultiplyScalar(half, Sse.MultiplyScalar(vectorX, squaredApproximation));

        // 计算 1.5 - 0.5 * x * yₙ²
        var subtractResult = Sse.SubtractScalar(onePointFive, tempValue);

        // 最终迭代计算：yₙ * (1.5 - 0.5 * x * yₙ²)
        var finalResult = Sse.MultiplyScalar(initialApproximation, subtractResult);

        // 将Vector128标量转换为普通float返回
        return finalResult.ToScalar();
    }
}
