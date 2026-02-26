namespace CSharp.Math;

public static partial class HighPerfMath
{
    public static partial class Sqrt
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
    }
}