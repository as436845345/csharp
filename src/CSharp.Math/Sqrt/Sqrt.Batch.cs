using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class Sqrt
{
    /// <summary>
    /// 批量计算数组的平方根倒数（纯软件魔数法，无硬件指令依赖）
    /// </summary>
    /// <param name="array">输入浮点数组（要求所有元素 > 0）</param>
    /// <returns>计算后的数组（直接修改原数组内存）</returns>
    /// <remarks>
    /// 1. 实现：遍历数组，对每个元素调用标量魔数法（Quake III）
    /// 2. 优势：全平台兼容，无需CPU指令集支持
    /// 3. 性能：比硬件指令版慢，但优于纯牛顿迭代
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeBatchFastInverseSquareRootUnsafe(float[] array)
    {
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        for (int i = 0; i < array.Length; i++)
        {
            var x = Unsafe.Add(ref start, i);
            Unsafe.Add(ref start, i) = ComputeScalarFastInverseSquareRoot(x);
        }

        return array;
    }

    /// <summary>
    /// 批量计算数组的平方根倒数（硬件加速版，自动降级兼容）
    /// </summary>
    /// <param name="array">输入浮点数组（要求所有元素 > 0）</param>
    /// <returns>计算后的数组（直接修改原数组内存）</returns>
    /// <remarks>
    /// 1. 优先级：AVX(256位) → SSE(128位) → 标量魔数法
    /// 2. 处理逻辑：
    ///    - 批量处理：256/128位整数倍长度的元素（硬件指令批量计算）
    ///    - 剩余元素：逐个调用标量硬件加速版计算
    /// 3. 性能：硬件指令批量处理比纯遍历快3-8倍（视数组长度）
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float[] ComputeBatchInverseSquareRootWithHardwareAcceleration(float[] array)
    {
        int length = array.Length;
        int offset = 0;
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        // 优先使用AVX（256位）批量计算
        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithAvx(ref start, ref offset, length);
        }
        // 降级使用SSE（128位）批量计算
        else if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            NewtonRaphson.ComputeBatchInverseSquareRootWithSse(ref start, ref offset, length);
        }

        // 处理剩余不足256/128位的元素（标量硬件加速版）
        for (; offset < length; offset++)
        {
            var x = Unsafe.Add(ref start, offset);
            Unsafe.Add(ref start, offset) = ComputeScalarInverseSquareRootWithHardwareAcceleration(x);
        }

        return array;
    }
}
