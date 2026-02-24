using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CSharp.Math.Sqrt;

[Display]
public class Sqrt
{
    /// <summary>
    /// 快速倒数平方根
    /// <code/>
    /// Fast 1/sqrt(x) using Quake III method
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FastInverseSquareRoot(float x)
    {
        int i = BitConverter.SingleToInt32Bits(x);
        i = 0x5f3759df - (i >> 1);
        float y = BitConverter.Int32BitsToSingle(i);

        // 牛顿迭代（Newton–Raphson）公式
        return y * (1.5f - 0.5f * x * y * y);
    }
}
