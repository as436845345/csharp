using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CSharp.Benchmark.Math;

// HardwareCounters 需要管理员运行 VS2022

[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
//[HardwareCounters(
//        HardwareCounter.BranchMispredictions,
//        HardwareCounter.BranchInstructions)]
public class InverseBenchmark : BenchmarkBase<InverseBenchmark>
{
    #region Scalar

    /// <summary>
    /// 提供基准测试用的浮点数输入序列 (2.0 ~ 9.0)
    /// Provides float input values for benchmarking (2.0 ~ 9.0)
    /// </summary>
    public static IEnumerable<float> FloatSource()
    {
        yield return 2f;
        yield return 3f;
        yield return 4f;
        yield return 5f;
        yield return 6f;
        yield return 7f;
        yield return 8f;
        yield return 9f;
    }

    /// <summary>
    /// 基准方法：使用原生除法计算倒数 (1/x)
    /// Baseline: compute reciprocal using native division (1/x)
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Scalar")]
    [ArgumentsSource(nameof(FloatSource))]
    public float Reciprocal_Naive(float x)
    {
        return 1f / x;
    }

    /// <summary>
    /// 优化方法：使用 SSE 指令 + 牛顿 - 拉夫逊迭代计算倒数
    /// 1. RCPSS 获取初始近似值 (约11位精度)
    /// 2. 一次牛顿迭代: y₁ = y₀ * (2 - x * y₀) 提升至约22位精度
    /// 
    /// Optimized: compute reciprocal using SSE + Newton-Raphson iteration
    /// 1. RCPSS for initial approximation (~11-bit precision)
    /// 2. One Newton-Raphson iteration: y₁ = y₀ * (2 - x * y₀) → ~22-bit precision
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Scalar")]
    [ArgumentsSource(nameof(FloatSource))] // 修复：原代码错误引用了 DivideByOne
    public float Reciprocal_SseNewtonRaphson(float x)
    {
        // 将标量 float 打包为 Vector128 (零开销抽象，便于 SIMD 指令操作)
        // Pack scalar float into Vector128 for SIMD operations (zero-cost abstraction)
        var vectorX = Vector128.CreateScalarUnsafe(x);

        // SSE 指令: _mm_rcp_ss - 硬件快速倒数近似 (精度约 2^-11)
        // SSE instruction: RCPSS - hardware reciprocal approximation (~2^-11 precision)
        var initialApprox = Sse.ReciprocalScalar(vectorX);

        // 预定义常量 2.0 用于牛顿迭代公式
        // Predefined constant 2.0 for Newton-Raphson formula
        var two = Vector128.CreateScalarUnsafe(2f);

        // 牛顿 - 拉夫逊迭代: error = 2 - x * y₀
        // Newton-Raphson iteration: error = 2 - x * y₀
        var errorTerm = Sse.SubtractScalar(two, Sse.MultiplyScalar(vectorX, initialApprox));

        // 牛顿 - 拉夫逊迭代: y₁ = y₀ * error = y₀ * (2 - x * y₀)
        // Newton-Raphson iteration: y₁ = y₀ * error = y₀ * (2 - x * y₀)
        var refinedResult = Sse.MultiplyScalar(initialApprox, errorTerm);

        // 提取 Vector128 中的标量结果并返回
        // Extract scalar result from Vector128 and return
        return refinedResult.ToScalar();
    }

    #endregion

    #region Vectorized

    public static IEnumerable<float[]> FloatsSource()
    {
        yield return CreateRandomArray(3444);
        yield return CreateRandomArray(7103);
        yield return CreateRandomArray(8887);
        yield return CreateRandomArray(10007);

        static float[] CreateRandomArray(int length)
        {
            var array = new float[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = Random.Shared.Next(1, 1011) * Random.Shared.NextSingle();
            }
            return array;
        }
    }

    /// <summary>
    /// [Baseline] Scalar: 逐个元素原生除法 (1/x)
    /// </summary>
    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Batch_Scalar(float[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 1 / array[i];
        }
    }

    /// <summary>
    /// SSE: 128-bit SIMD + Newton-Raphson (4 floats/iter)
    /// Fallback: scalar for remainder
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Vectorized_Sse(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var two = Vector128.Create(2f);

            do
            {
                var vector = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var initialApprox = Sse.Reciprocal(vector);
                var errorTerm = Sse.Subtract(two, Sse.Multiply(vector, initialApprox));
                var refinedResult = Sse.Multiply(initialApprox, errorTerm);

                Vector128.StoreUnsafe(refinedResult, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var val = ref Unsafe.Add(ref start, offset);
            val = 1 / val;
        }
    }

    /// <summary>
    /// AVX: 256-bit SIMD + Newton-Raphson (8 floats/iter)
    /// Fallback: scalar for remainder
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Vectorized_Avx(float[] array)
    {
        var length = array.Length;
        var offset = 0;

        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var two = Vector256.Create(2f);

            do
            {
                var vector = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var initialApprox = Avx.Reciprocal(vector);
                var errorTerm = Avx.Subtract(two, Avx.Multiply(vector, initialApprox));
                var refinedResult = Avx.Multiply(initialApprox, errorTerm);

                Vector256.StoreUnsafe(refinedResult, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var val = ref Unsafe.Add(ref start, offset);
            val = 1 / val;
        }
    }

    /// <summary>
    /// Hybrid: AVX primary + SSE fallback + scalar tail
    /// 优先 AVX, 剩余用 SSE, 最后标量补齐
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Vectorized_AvxSseHybrid(float[] array)
    {
        var length = array.Length;
        var offset = 0;
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        // Stage 1: AVX 256-bit
        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var two = Vector256.Create(2f);
            do
            {
                var v = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var approx = Avx.Reciprocal(v);
                var error = Avx.Subtract(two, Avx.Multiply(v, approx));
                var result = Avx.Multiply(approx, error);
                Vector256.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }

        // Stage 2: SSE 128-bit fallback for remaining data
        if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            var two = Vector128.Create(2f);
            do
            {
                var v = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var approx = Sse.Reciprocal(v);
                var error = Sse.Subtract(two, Sse.Multiply(v, approx));
                var result = Sse.Multiply(approx, error);
                Vector128.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        // Stage 3: Scalar tail
        for (; offset < length; offset++)
        {
            ref var val = ref Unsafe.Add(ref start, offset);
            val = 1f / val;
        }
    }

    /// <summary>
    /// Hybrid (if-else): AVX or SSE (exclusive) + scalar tail
    /// 根据 CPU 支持选择 AVX 或 SSE，二者互斥
    /// </summary>
    [Benchmark]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Vectorized_AvxOrSse(float[] array)
    {
        var length = array.Length;
        var offset = 0;
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var two = Vector256.Create(2f);
            do
            {
                var v = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var approx = Avx.Reciprocal(v);
                var error = Avx.Subtract(two, Avx.Multiply(v, approx));
                var result = Avx.Multiply(approx, error);
                Vector256.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }
        else if (Sse.IsSupported && length >= Vector128<float>.Count)
        {
            var two = Vector128.Create(2f);
            do
            {
                var v = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var approx = Sse.Reciprocal(v);
                var error = Sse.Subtract(two, Sse.Multiply(v, approx));
                var result = Sse.Multiply(approx, error);
                Vector128.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var val = ref Unsafe.Add(ref start, offset);
            val = 1f / val;
        }
    }

    [Benchmark]
    [BenchmarkCategory("Vectorized")]
    [ArgumentsSource(nameof(FloatsSource))]
    public void Vectorized_AvxOrSse_Divide(float[] array)
    {
        var length = array.Length;
        var offset = 0;
        ref var start = ref MemoryMarshal.GetArrayDataReference(array);

        if (Avx.IsSupported && length >= Vector256<float>.Count)
        {
            var two = Vector256.Create(2f);
            do
            {
                var v = Vector256.LoadUnsafe(ref start, (nuint)offset);
                var result = Avx.Divide(Vector256<float>.One, v);
                Vector256.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector256<float>.Count) >= Vector256<float>.Count);
        }
        else if (Sse.IsSupported && length - offset >= Vector128<float>.Count)
        {
            var two = Vector128.Create(2f);
            do
            {
                var v = Vector128.LoadUnsafe(ref start, (nuint)offset);
                var result = Sse.Divide(Vector128<float>.One, v);
                Vector128.StoreUnsafe(result, ref start, (nuint)offset);
            } while (length - (offset += Vector128<float>.Count) >= Vector128<float>.Count);
        }

        for (; offset < length; offset++)
        {
            ref var val = ref Unsafe.Add(ref start, offset);
            val = 1f / val;
        }
    }

    #endregion
}