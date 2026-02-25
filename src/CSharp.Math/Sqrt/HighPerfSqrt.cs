using System.ComponentModel.DataAnnotations;

namespace CSharp.Math;

[Display]
public static partial class HighPerfSqrt
{
    public static void Execute()
    {
        var x = 1311.22f;
        var v1 = ComputeScalarFastInverseSquareRoot(x);
        var sse_1 = ComputeScalarInverseSquareRootWithHardwareAcceleration(x);

        var array = new float[37];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
        }

        var f2_v = ComputeBatchFastInverseSquareRootUnsafe(array.AsSpan().ToArray());
        var avx3_v = ComputeBatchInverseSquareRootWithHardwareAcceleration(array.AsSpan().ToArray());
    }
}
