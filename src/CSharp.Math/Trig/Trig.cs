using System.Runtime.CompilerServices;

namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static class Trig
    {
        internal static void Execute()
        {
            Console.WriteLine("===============================[Trig]===============================");

            var array = new float[] { PI / 6, 90f, 180f, 359f };

            foreach (float f in array)
            {
                Console.WriteLine($"------------------{f}------------------");
                var mfSinX = MathF.Sin(f);
                var sinX = Sin(f);
                var st = 1 - sinX / mfSinX;
                Console.WriteLine($"MathF.Sin({f}) = {mfSinX}");
                Console.WriteLine($"Sin({f}) = {sinX}");
                Console.WriteLine($"差值：{st}");

                Console.WriteLine();

                var mfCosX = MathF.Cos(f);
                var cosX = Cos(f);
                var ct = 1 - cosX / mfCosX;
                Console.WriteLine($"MathF.Cos({f}) = {mfCosX}");
                Console.WriteLine($"Cos({f}) = {cosX}");
                Console.WriteLine($"差值：{ct}");
            }

            Console.WriteLine("====================================================================");
        }

        /// <summary>
        /// 快速弧度规约：将任意弧度映射至 [-π, π] 区间。
        /// </summary>
        /// <remarks>
        /// 当 |x| &lt; 10000 时精度较高。对于极大值，浮点数舍入误差会显著增加，建议使用高精度规约。
        /// 原理：x_reduced = x - round(x / 2π) * 2π
        /// </remarks>
        /// <param name="x">原始弧度</param>
        /// <returns>规约后的弧度，范围在 [-π, π] 之间</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ReduceAngle(float x)
        {
            // 核心逻辑：找出 x 包含了多少个完整的 2π 周期，并减去它们
            float n = MathF.Round(x * INV_TWO_PI);
            return x - n * TWO_PI;
        }

        /// <summary>
        /// 快速正弦函数 (Sin)。
        /// 基于泰勒级数展开：sin(x) ≈ x - x³/3! + x⁵/5! - x⁷/7!
        /// </summary>
        /// <param name="x">弧度</param>
        /// <returns>sin(x) 的近似值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float x)
        {
            // 1. 将角度收缩到泰勒级数拟合效果最好的范围 [-π, π]
            x = ReduceAngle(x);
            float x2 = x * x;

            // 2. 霍纳法则 (Horner's Method) 计算多项式，减少乘法次数
            // 系数说明：
            // -0.16666667f ≈ -1/6 (1/3!)
            // 0.008333333f ≈ 1/120 (1/5!)
            // -0.0001984127f ≈ -1/5040 (1/7!)
            return x * (1f + x2 * (-0.16666667f + x2 * (0.008333333f + x2 * -0.0001984127f)));
        }

        /// <summary>
        /// 快速余弦函数 (Cos)。
        /// 基于泰勒级数展开：cos(x) ≈ 1 - x²/2! + x⁴/4! - x⁶/6!
        /// </summary>
        /// <param name="x">弧度</param>
        /// <returns>cos(x) 的近似值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float x)
        {
            // 1. 角度规约
            x = ReduceAngle(x);
            float x2 = x * x;

            // 2. 泰勒级数近似
            // 系数说明：
            // -0.5f = -1/2 (1/2!)
            // 0.041666667f ≈ 1/24 (1/4!)
            // -0.001388889f ≈ -1/720 (1/6!)
            return 1f + x2 * (-0.5f + x2 * (0.041666667f + x2 * -0.001388889f));
        }

        /// <summary>
        /// 快速正余弦同步计算 (SinCos)。
        /// </summary>
        /// <remarks>
        /// 性能：比分别调用 Sin(x) 和 Cos(x) 更快，因为减少了一次角度规约 (ReduceAngle) 的开销。
        /// 精度：此处为了极致性能，泰勒级数阶数比单独的 Sin/Cos 函数略低（仅到 x^5 和 x^4 级别）。
        /// 适用于：对精度要求不高但需要同时获取两个值的场景，如 2D 旋转矩阵计算。
        /// </remarks>
        /// <param name="x">弧度</param>
        /// <returns>包含 sin 和 cos 结果的元组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (float sin, float cos) SinCos(float x)
        {
            // 1. 统一进行一次角度规约，减少重复浮点运算
            x = ReduceAngle(x);
            float x2 = x * x;

            // 2. 泰勒级数展开（低阶近似）：
            // Sin ≈ x - x^3/3! + x^5/5!
            float sin = x * (1f + x2 * (-0.16666667f + x2 * 0.008333333f));
            // Cos ≈ 1 - x^2/2! + x^4/4!
            float cos = 1f + x2 * (-0.5f + x2 * 0.041666667f);

            return (sin, cos);
        }

        /// <summary>
        /// 快速正切函数 (Tan)。
        /// </summary>
        /// <remarks>
        /// 实现原理：tan(x) = sin(x) / cos(x)。
        /// 注意：由于使用泰勒级数近似，在靠近 π/2 (90°) 或 -π/2 (-90°) 时，
        /// cos 的近似值可能趋近于 0，导致结果偏差增大甚至出现除零风险。
        /// </remarks>
        /// <param name="x">弧度</param>
        /// <returns>tan(x) 的近似值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Tan(float x)
        {
            // 调用同步计算方法，最大化代码复用和指令效率
            var (sin, cos) = SinCos(x);
            return sin / cos;
        }
    }
}

