using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Log2
    {
        /// <summary>
        /// 基于多项式逼近的快速 Log2 计算。
        /// <para>原理：提取 IEEE 754 阶码 e，并对尾数 m 在 [1, 2) 区间使用三阶多项式拟合 log2(m)。</para>
        /// <para>精度：最大误差约为 0.5%，适用于大多数对精度有一定要求但追求性能的场景（如音频处理、物理模拟）。</para>
        /// </summary>
        /// <param name="x">输入值（必须为正数）。</param>
        /// <returns>log2(x) 的近似值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Approximated(float x)
        {
            // IEEE 754: x = 2^e * m where m in [1, 2)
            int i = BitConverter.SingleToInt32Bits(x);

            // 1. 提取阶码 (Exponent)
            int e = ((i >> 23) & 0xFF) - 127;

            // 2. 提取尾数 (Mantissa)，并将其归一化到 [1, 2) 区间
            // 0x3F800000 是浮点数 1.0f 的位表示
            int mantissaBits = (i & 0x7FFFFF) | 0x3F800000;
            float m = BitConverter.Int32BitsToSingle(mantissaBits);

            // 3. 使用秦九韶算法计算多项式: t * (a - t * (b - t * c))
            // 拟合 log2(1 + t), t 属于 [0, 1)
            float t = m - 1f;
            float log2m = t * (1.4425f - t * (0.7213f - t * 0.4825f));

            // 根据对数运算法则: log2(2^e * m) = e + log2(m)
            return e + log2m;
        }

        /// <summary>
        /// 基于位模式线性映射的极速 Log2 估算。
        /// <para>原理：利用浮点数内存布局在局部区间与对数曲线相似的特性，通过一次减法和乘法直接完成估算。</para>
        /// <para>精度：最大误差约为 5%，精度较低，但速度极快（约 4 个时钟周期）。</para>
        /// </summary>
        /// <param name="x">输入值（必须为正数）。</param>
        /// <returns>log2(x) 的粗略估算值。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Estimated(float x)
        {
            int i = BitConverter.SingleToInt32Bits(x);
            // 1065353216 = 127 << 23 (即 1.0f 的整数表示)
            // 8.2629...e-8f = 1 / 2^23 的修正值
            return (i - 1065353216) * 8.2629582881927490e-8f;
        }
    }
}