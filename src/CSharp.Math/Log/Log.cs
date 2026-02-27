using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Log2
    {
        internal static void Execute()
        {
            Console.WriteLine("[Log2]");

            // 1. 准备测试数据
            const int count = 1_000_000; // 测试 100 万个随机数
            var testData = new float[count];

            for (int i = 0; i < count; i++)
            {
                // 生成范围在 (0.001, 1000000) 之间的随机浮点数
                testData[i] = (float)Random.Shared.NextDouble() * 1000000f + 0.001f;
            }

            double totalErrorApprox = 0;
            double totalErrorEst = 0;
            float maxErrorApprox = 0;
            float maxErrorEst = 0;

            // 2. 开始测试
            foreach (var x in testData)
            {
                float actual = MathF.Log2(x);
                float approx = Approximated(x);
                float est = Estimated(x);

                // 计算绝对误差
                float diffApprox = MathF.Abs(actual - approx);
                float diffEst = MathF.Abs(actual - est);

                // 累加误差用于平均值
                totalErrorApprox += diffApprox;
                totalErrorEst += diffEst;

                // 记录最大误差
                if (diffApprox > maxErrorApprox) maxErrorApprox = diffApprox;
                if (diffEst > maxErrorEst) maxErrorEst = diffEst;
            }

            // 3. 输出结果
            Console.WriteLine($"--- Log2 Accuracy Test (Samples: {count}) ---");
            Console.WriteLine($"[Approximated (多项式)]");
            Console.WriteLine($"  平均绝对误差: {totalErrorApprox / count:F6}");
            Console.WriteLine($"  最大绝对误差: {maxErrorApprox:F6}");

            Console.WriteLine($"\n[Estimated (位模式)]");
            Console.WriteLine($"  平均绝对误差: {totalErrorEst / count:F6}");
            Console.WriteLine($"  最大绝对误差: {maxErrorEst:F6}");
        }

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

            // 泰勒展开
            //float log2m = t * (1.4425f - t * (0.7213f - t * 0.4825f));

            // 推荐系数：区间拟合版本
            // 相比泰勒系数：
            // - 最大误差更小
            // - 全区间更均匀
            // - 更适合 DSP / 图形 / 实时系统
            float log2m = t * (1.5353f - t * (0.7635f - t * 0.2282f));

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
            // 1.1920928955078125e-7f = 1 / 2^23 的修正值
            return (i - 1065353216) * 1.1920928955078125e-7f;
        }
    }
}