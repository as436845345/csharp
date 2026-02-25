using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CSharp.Math;

[Display]
public static partial class HighPerfMath
{
    public static void Execute()
    {
        Sqrt.Execute();
    }

    /// <summary>
    /// 计算单精度浮点数的倒数（1/x）：基于SSE指令集+牛顿-拉夫逊迭代法
    /// </summary>
    /// <param name="x">输入值（要求 x ≠ 0）</param>
    /// <returns>1/x 的高精度近似值</returns>
    /// <remarks>
    /// 1. 底层实现：调用 NewtonRaphson.ComputeScalarInverseWithSse 方法
    /// 2. 性能优化：启用 AggressiveInlining 内联，消除方法调用开销
    /// 3. 指令集适配：优先使用SSE的ReciprocalScalar指令获取初始值，牛顿迭代优化精度
    /// 4. 适用场景：高性能计算中替代除法运算（乘法指令耗时远低于除法）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ComputeScalarInverse(float x)
    {
        return NewtonRaphson.ComputeScalarInverseWithSse(x);
    }
}
