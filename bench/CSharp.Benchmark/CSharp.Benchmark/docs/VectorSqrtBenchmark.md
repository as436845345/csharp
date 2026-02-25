## 性能测试

| Method                                   | array      | Mean      | Error     | StdDev    | Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
|----------------------------------------- |----------- |----------:|----------:|----------:|------:|--------:|----------:|----------:|------------:|
| ComputeVectorFastInverseSquareRoot       | Single[83] | 80.913 ns | 0.4246 ns | 0.3764 ns |  1.00 |    0.01 |      98 B |         - |          NA |
| ComputeVectorFastInverseSquareRootUnsafe | Single[83] | 85.728 ns | 0.5179 ns | 0.4844 ns |  1.06 |    0.01 |     103 B |         - |          NA |
| ComputeSseInverseSqrtNewton              | Single[83] | 15.569 ns | 0.1952 ns | 0.1825 ns |  0.19 |    0.00 |     156 B |         - |          NA |
| ComputeSseInverseSqrtDivide              | Single[83] | 29.984 ns | 0.3058 ns | 0.2861 ns |  0.37 |    0.00 |     113 B |         - |          NA |
| ComputeAvxInverseSqrtNewton              | Single[83] |  8.684 ns | 0.1965 ns | 0.1742 ns |  0.11 |    0.00 |     159 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtNewton     | Single[83] | 10.182 ns | 0.1550 ns | 0.1450 ns |  0.13 |    0.00 |     249 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtNewton   | Single[83] | 10.197 ns | 0.1135 ns | 0.1061 ns |  0.13 |    0.00 |     246 B |         - |          NA |
| ComputeAvxInverseSqrtDivide              | Single[83] | 23.406 ns | 0.1112 ns | 0.0986 ns |  0.29 |    0.00 |     131 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtDivide     | Single[83] | 23.345 ns | 0.1267 ns | 0.1124 ns |  0.29 |    0.00 |     192 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtDivide   | Single[83] | 23.333 ns | 0.1007 ns | 0.0841 ns |  0.29 |    0.00 |     189 B |         - |          NA |
| ComputeVectorFastInverseSquareRoot       | Single[84] | 82.823 ns | 1.6154 ns | 1.7955 ns |  1.02 |    0.02 |      98 B |         - |          NA |
| ComputeVectorFastInverseSquareRootUnsafe | Single[84] | 89.364 ns | 0.7872 ns | 0.7364 ns |  1.10 |    0.01 |     103 B |         - |          NA |
| ComputeSseInverseSqrtNewton              | Single[84] | 14.867 ns | 0.1877 ns | 0.1664 ns |  0.18 |    0.00 |     156 B |         - |          NA |
| ComputeSseInverseSqrtDivide              | Single[84] | 27.375 ns | 0.5093 ns | 0.4515 ns |  0.34 |    0.01 |     113 B |         - |          NA |
| ComputeAvxInverseSqrtNewton              | Single[84] |  9.477 ns | 0.1093 ns | 0.0913 ns |  0.12 |    0.00 |     159 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtNewton     | Single[84] |  8.188 ns | 0.0311 ns | 0.0243 ns |  0.10 |    0.00 |     247 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtNewton   | Single[84] | 11.231 ns | 0.1594 ns | 0.1491 ns |  0.14 |    0.00 |     246 B |         - |          NA |
| ComputeAvxInverseSqrtDivide              | Single[84] | 24.326 ns | 0.1994 ns | 0.1865 ns |  0.30 |    0.00 |     131 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtDivide     | Single[84] | 26.513 ns | 0.5117 ns | 0.4787 ns |  0.33 |    0.01 |     185 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtDivide   | Single[84] | 23.320 ns | 0.1256 ns | 0.1175 ns |  0.29 |    0.00 |     189 B |         - |          NA |
| ComputeVectorFastInverseSquareRoot       | Single[86] | 82.631 ns | 0.3856 ns | 0.3220 ns |  1.02 |    0.01 |      98 B |         - |          NA |
| ComputeVectorFastInverseSquareRootUnsafe | Single[86] | 87.554 ns | 0.2857 ns | 0.2385 ns |  1.08 |    0.01 |     103 B |         - |          NA |
| ComputeSseInverseSqrtNewton              | Single[86] | 15.922 ns | 0.2225 ns | 0.1973 ns |  0.20 |    0.00 |     156 B |         - |          NA |
| ComputeSseInverseSqrtDivide              | Single[86] | 29.783 ns | 0.1942 ns | 0.1622 ns |  0.37 |    0.00 |     113 B |         - |          NA |
| ComputeAvxInverseSqrtNewton              | Single[86] | 10.224 ns | 0.1378 ns | 0.1222 ns |  0.13 |    0.00 |     159 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtNewton     | Single[86] |  9.411 ns | 0.1117 ns | 0.0933 ns |  0.12 |    0.00 |     244 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtNewton   | Single[86] | 12.625 ns | 0.0649 ns | 0.0607 ns |  0.16 |    0.00 |     246 B |         - |          NA |
| ComputeAvxInverseSqrtDivide              | Single[86] | 23.287 ns | 0.1253 ns | 0.1172 ns |  0.29 |    0.00 |     131 B |         - |          NA |
| ComputeAvxSseHybridInverseSqrtDivide     | Single[86] | 25.870 ns | 0.0851 ns | 0.0755 ns |  0.32 |    0.00 |     185 B |         - |          NA |
| ComputeAvxFallbackSseInverseSqrtDivide   | Single[86] | 23.246 ns | 0.1002 ns | 0.0888 ns |  0.29 |    0.00 |     189 B |         - |          NA |

根据测试，优先使用 `ComputeAvxInverseSqrtNewton`。

## 汇编

[VectorSqrtBenchmark-asm.md](../bin/Release/net8.0/BenchmarkDotNet.Artifacts/results/CSharp.Benchmark.Math.VectorSqrtBenchmark-asm.md)