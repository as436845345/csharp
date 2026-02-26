using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 对 1/x 的初始近似值执行一次牛顿 - 拉夫逊迭代优化
    /// </summary>
    /// <param name="x">目标值（x ≠ 0）</param>
    /// <param name="initialApprox">初始近似值（建议由 <see cref="Sse.ReciprocalScalar(Vector128{float})"/> 等指令提供，精度 ~11 位）</param>
    /// <returns>优化后的近似值（精度 ~22 位，相对误差 &lt; 2⁻²²）</returns>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>公式：y₁ = y₀ × (2 - x × y₀)</description></item>
    ///   <item><description>收敛：二次收敛，1 次迭代即可达 float 精度上限</description></item>
    ///   <item><description>用途：硬件近似指令的后处理，提升精度</description></item>
    /// </list>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RefineReciprocal(float x, float initialApprox)
    {
        return initialApprox * (2 - x * initialApprox);
    }

    /// <summary>
    /// 对 1/√x 的初始近似值执行一次牛顿 - 拉夫逊迭代优化
    /// </summary>
    /// <param name="x">目标值（x > 0）</param>
    /// <param name="initialApprox">初始近似值（建议由 <see cref="Sse.ReciprocalSqrtScalar(Vector128{float})"/> 或魔数法提供）</param>
    /// <returns>优化后的近似值（精度 ~23 位）</returns>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>📐 公式：y₁ = y₀ × (1.5 - 0.5 × x × y₀²)</description></item>
    ///   <item><description>🎮 用途：图形学中向量归一化、光照计算的核心优化</description></item>
    /// </list>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float RefineInverseSqrt(float x, float initialApprox)
    {
        return initialApprox * (1.5f - 0.5f * x * initialApprox * initialApprox);
    }
}
