using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 使用 SSE + 牛顿迭代计算 1/x（倒数）
    /// 精度：~22 位（单精度 float 上限），延迟：~15 周期
    /// </summary>
    /// <param name="x">输入值（x ≠ 0）</param>
    /// <returns>1/x 的高精度近似值</returns>
    /// <remarks>
    /// 1. 牛顿迭代公式：yₙ₊₁ = yₙ * (2 - x * yₙ)
    /// 2. 纯SSE硬件指令加速，无托管代码开销，适用于高性能数值计算场景
    /// 3. 二次收敛特性，单次迭代即可将初始近似值精度提升至单精度浮点数上限
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReciprocalSse(float x)
    {
        var vx = Vector128.CreateScalarUnsafe(x);
        var approx = Sse.ReciprocalScalar(vx);
        var two = Vector128.CreateScalarUnsafe(2f);
        var error = Sse.SubtractScalar(two, Sse.MultiplyScalar(vx, approx));
        return Sse.MultiplyScalar(approx, error).ToScalar();
    }

    /// <summary>
    /// 使用 SSE + 牛顿迭代计算 1/√x（平方根倒数）
    /// 精度：~23 位，延迟：~18 周期
    /// 常用于：向量归一化、光照计算、物理引擎
    /// </summary>
    /// <param name="x">输入值（x > 0）</param>
    /// <returns>1/√x 的高精度近似值</returns>
    /// <remarks>
    /// 1. 先通过SSE的ReciprocalSqrtScalar获取初始粗糙近似值
    /// 2. 再通过牛顿迭代公式优化精度
    /// 3. 纯硬件指令加速，适用于高性能计算场景（如图形学、游戏引擎）
    /// </remarks>
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
}
