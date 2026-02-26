using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 标量：使用 SSE + 牛顿迭代计算 1/√x（平方根倒数）
    /// </summary>
    /// <param name="x">输入值（x > 0）</param>
    /// <returns>1/√x 的高精度近似值（精度 ~23 位）</returns>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>公式：y₁ = y₀ × (1.5 - 0.5 × x × y₀²)</description></item>
    ///   <item><description>性能：延迟 ~18 周期，常用于向量归一化、光照计算</description></item>
    ///   <item><description>场景：游戏引擎、物理模拟、图形学</description></item>
    /// </list>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float InverseSqrtScalarSse(float x)
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
