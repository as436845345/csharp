using BenchmarkDotNet.Attributes;
using CSharp.Math.Sqrt;

namespace CSharp.Benchmark.Math;

// MathSqrt_Bench & FastInverseSquareRoot_Bench
//
//| Method                      | x   | Mean      | Error     | StdDev    | Ratio | Code Size | Allocated | Alloc Ratio |
//|---------------------------- |---- |----------:|----------:|----------:|------:|----------:|----------:|------------:|
//| MathSqrt_Bench              | 2   | 5.1189 ns | 0.0320 ns | 0.0283 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 2   | 0.2387 ns | 0.0183 ns | 0.0171 ns |  0.05 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 2.5 | 5.0682 ns | 0.0394 ns | 0.0329 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 2.5 | 0.2312 ns | 0.0107 ns | 0.0100 ns |  0.05 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 3   | 5.0950 ns | 0.0350 ns | 0.0327 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 3   | 0.2393 ns | 0.0069 ns | 0.0062 ns |  0.05 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 4   | 3.9161 ns | 0.0324 ns | 0.0303 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 4   | 0.2807 ns | 0.0298 ns | 0.0417 ns |  0.07 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 5   | 5.3190 ns | 0.0178 ns | 0.0148 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 5   | 0.2313 ns | 0.0247 ns | 0.0231 ns |  0.04 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 6   | 5.2418 ns | 0.0172 ns | 0.0153 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 6   | 0.2467 ns | 0.0069 ns | 0.0058 ns |  0.05 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 7   | 5.0613 ns | 0.0356 ns | 0.0333 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 7   | 0.2368 ns | 0.0073 ns | 0.0061 ns |  0.05 |      53 B |         - |          NA |
//|                             |     |           |           |           |       |           |           |             |
//| MathSqrt_Bench              | 10  | 5.0561 ns | 0.0289 ns | 0.0256 ns |  1.00 |      16 B |         - |          NA |
//| FastInverseSquareRoot_Bench | 10  | 0.2350 ns | 0.0108 ns | 0.0090 ns |  0.05 |      53 B |         - |          NA |

public class SqrtBench : BenchBase<SqrtBench>
{
    public static IEnumerable<float> FloatSource()
    {
        yield return 2f;
        yield return 2.5f;
        yield return 3f;
        yield return 4f;
        yield return 5f;
        yield return 6f;
        yield return 7f;
        yield return 10f;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(FloatSource))]
    public float MathSqrt_Bench(float x)
    {
        return (float)System.Math.Sqrt(x);
    }

    [Benchmark]
    [ArgumentsSource(nameof(FloatSource))]
    public float FastInverseSquareRoot_Bench(float x)
    {
        return Sqrt.FastInverseSquareRoot(x);
    }
}
