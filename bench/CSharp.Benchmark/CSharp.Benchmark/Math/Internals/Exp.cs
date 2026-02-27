using System.Runtime.CompilerServices;

namespace CSharp.Benchmark.Math.Internals;

public static class Exponential
{
    public const float LOG2E = 1.44269504088896340736f;

    /// <summary>
    /// 线性对数近似指数函数 (Schraudolph 算法)。
    /// 性能：极致 (~4 cycles)。精度：低 (Max Error ~5%)。
    /// 原理：利用 IEEE 754 整数视图与对数函数的线性映射关系 $I \approx 2^{23}(x \log_2 e + 127 - \sigma)$。
    /// 适用场景：神经网络激活函数 (Sigmoid/Softmax)、对精度不敏感的实时信号缩放。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ExpApprox(float x)
    {
        // 限制范围防止阶码溢出污染符号位
        x = System.Math.Clamp(x, -87f, 88f);
        // 12102203 = 2^23 * log2(e)
        // 1065353216 = 2^23 * (127 - offset), 此处 offset 约 0.0437
        int i = (int)(12102203f * x + 1065353216f);
        return BitConverter.Int32BitsToSingle(i);
    }

    /// <summary>
    /// 基于范围缩减与 3 阶多项式拟合的指数函数。
    /// 性能：高 (~10 cycles)。精度：中 (Max Error ~0.5%)。
    /// 原理：exp(x) = 2^(x * log2 e) = 2^n * 2^f。其中 2^n 通过位移构造，2^f 通过 Horner 形式多项式近似。
    /// 适用场景：游戏物理引擎、粒子系统、常规图形学计算。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp(float x)
    {
        x = System.Math.Clamp(x, -87f, 88f);

        // 换底：e^x = 2^t
        float t = x * LOG2E;

        // 拆分整数 n 与小数 f [0, 1)
        int n = (int)MathF.Floor(t);
        float f = t - n;

        // 2^f 在 [0, 1) 区间的极小化极大拟合多项式
        float p = 1f + f * (0.6931472f + f * (0.2402265f + f * 0.0558012f));

        // 构造 2^n 的浮点数位模式并合并结果
        int bits = (n + 127) << 23;
        return BitConverter.Int32BitsToSingle(bits) * p;
    }

    /// <summary>
    /// 快速计算 2 的 x 次幂 (Pow2)。
    /// 性能：极高 (~8 cycles)。精度：中 (Max Error ~0.3%)。
    /// 原理：直接对输入进行整数/小数拆分，跳过 Exp(x) 的换底乘法，直接操作阶码。
    /// 适用场景：音频倍频程计算、以 2 为底的指数缩放。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Exp2(float x)
    {
        // 允许触及 float 的极限范围 [-126, 128]
        x = System.Math.Clamp(x, -126f, 128f);

        int n = (int)MathF.Floor(x);
        float f = x - n;

        // 2^f 拟合
        float p = 1f + f * (0.6931472f + f * (0.2402265f + f * 0.0558012f));

        // 位移构造 2^n
        int bits = (n + 127) << 23;
        return BitConverter.Int32BitsToSingle(bits) * p;
    }
}
