using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Sqrt
    {
        internal static void Execute()
        {
            var x = 1311.22f;
            var isq_value = InverseSqrtMagic(x);
            var is_value = InverseSqrt(x);

            var array = new float[37];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
            }

            var a = array.AsSpan().ToArray();
            var b = array.AsSpan().ToArray();
            InverseSqrtBatchMagic(a);
            InverseSqrtBatch(b);
        }

        #region Scalar

        /// <summary>
        /// 标量：Quake III 魔数法计算 1/√x（全平台兼容）
        /// </summary>
        /// <param name="x">输入值（x > 0）</param>
        /// <returns>1/√x 的近似值（相对误差 ~1e-6）</returns>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>来源：Quake III Arena 源码，0x5f3759df 魔数</description></item>
        ///   <item><description>性能：无硬件依赖，延迟 ~12 周期</description></item>
        ///   <item><description>精度：单次牛顿迭代，误差约 0.001%</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseSqrtMagic(float x)
        {
            int i = BitConverter.SingleToInt32Bits(x);
            i = 0x5f3759df - (i >> 1);
            float y = BitConverter.Int32BitsToSingle(i);
            return NewtonRaphson.RefineInverseSqrt(x, y);
        }

        /// <summary>
        /// 标量：计算 1/√x，自动选择最优后端（SSE 优先，魔数法降级）
        /// </summary>
        /// <param name="x">输入值（x > 0）</param>
        /// <returns>1/√x 的高精度近似值（精度 ~23 位）</returns>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>策略：Sse.IsSupported ? SSE 指令 : <see cref="InverseSqrtMagic"/></description></item>
        ///   <item><description>性能：SSE 版延迟 ~18 周期，比魔数法快 20-30%</description></item>
        ///   <item><description>推荐：作为默认入口，无需关心底层指令集</description></item>
        /// </list>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseSqrt(float x)
        {
            return Sse.IsSupported
                ? NewtonRaphson.InverseSqrtSse(x)
                : InverseSqrtMagic(x);
        }

        #endregion

        #region Batch

        /// <summary>
        /// 批量：魔数法计算 1/√x，全平台兼容，原地修改
        /// </summary>
        /// <param name="values">输入/输出数据（所有元素要求 > 0）</param>
        /// <remarks>
        /// <list type="bullet">
        ///   <item><description>实现：遍历 + 标量魔数法，无 SIMD 依赖</description></item>
        ///   <item><description>兼容：ARM/x86/x64 全平台可用</description></item>
        ///   <item><description>性能：适合小数组或无 SIMD 环境</description></item>
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

        #endregion
    }
}