using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static partial class Sqrt
    {
        /// <summary>
        /// 批量：魔数法计算 1/√x，全平台兼容，原地修改
        /// </summary>
        /// <param name="values">输入/输出数据（所有元素要求 > 0）</param>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>🔧 实现：遍历 + 标量魔数法，无 SIMD 依赖</description></item>
        ///   <item><description>🌍 兼容：ARM/x86/x64 全平台可用</description></item>
        ///   <item><description>⚡ 性能：适合小数组或无 SIMD 环境</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InverseSqrtBatchMagic(Span<float> values)
        {
            ref var start = ref MemoryMarshal.GetReference(values);

            for (int i = 0; i < values.Length; i++)
            {
                // values[i] = InverseSqrtMagic(values[i]);
                // 我记得有边界检测

                ref var x = ref Unsafe.Add(ref start, i);
                x = InverseSqrtMagic(x);
            }
        }

        /// <summary>
        /// 批量：计算 1/√x，自动选择最优指令集，原地修改
        /// </summary>
        /// <param name="values">输入/输出数据（所有元素要求 > 0）</param>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>策略：AVX(8 元素) → SSE(4 元素) → 标量魔数法(尾部)</description></item>
        ///   <item><description>性能拐点：长度 ≥ 8 时优于纯标量；≥ 64 时加速 3-8 倍</description></item>
        ///   <item><description>精度：同 <see cref="InverseSqrt(float)"/></description></item>
        ///   <item><description>注意：原地修改，非线程安全</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InverseSqrtBatch(Span<float> values)
        {
            int length = values.Length;
            int offset = 0;
            ref var start = ref MemoryMarshal.GetReference(values);

            // 优先使用AVX（256位）批量计算
            if (Avx.IsSupported && length >= Vector256<float>.Count)
            {
                NewtonRaphson.InverseSqrtBatchAvx(ref start, ref offset, length);
            }
            // 降级使用SSE（128位）批量计算
            else if (Sse.IsSupported && length >= Vector128<float>.Count)
            {
                NewtonRaphson.InverseSqrtBatchSse(ref start, ref offset, length);
            }

            // 处理剩余不足256/128位的元素（标量硬件加速版）
            for (; offset < length; offset++)
            {
                ref var x = ref Unsafe.Add(ref start, offset);
                x = InverseSqrt(x);
            }
        }
    }
}