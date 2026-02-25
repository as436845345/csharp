using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class NewtonRaphson
{
    /// <summary>
    /// 牛顿-拉夫逊迭代法计算 1/x（倒数）
    /// </summary>
    /// <param name="x">输入值（要求 x ≠ 0）</param>
    /// <param name="y">初始近似值（建议使用硬件指令如ReciprocalScalar获取粗糙近似值）</param>
    /// <returns>1/x 的高精度近似值</returns>
    /// <remarks>
    /// 迭代公式：yₙ₊₁ = yₙ * (2 - x * yₙ)
    /// 二次收敛特性，1-2次迭代即可达到单精度浮点数的精度上限
    /// 适用场景：高性能计算中替代除法运算（除法指令耗时高于乘法）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ComputeInverse(float x, float y)
    {
        return y * (2 - x * y);
    }

    /// <summary>
    /// 牛顿-拉夫逊迭代法计算 1/√x（平方根倒数）
    /// </summary>
    /// <param name="x">输入值（要求 x > 0）</param>
    /// <param name="y">初始近似值（建议使用硬件指令/魔数快速获取的粗糙近似值）</param>
    /// <returns>1/√x 的高精度近似值</returns>
    /// <remarks>
    /// 迭代公式：yₙ₊₁ = yₙ * (1.5 - 0.5 * x * yₙ²)
    /// 二次收敛特性，仅需1-2次迭代即可获得高精度结果
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ComputeInverseSquareRoot(float x, float y)
    {
        return y * (1.5f - 0.5f * x * y * y);
    }
}
