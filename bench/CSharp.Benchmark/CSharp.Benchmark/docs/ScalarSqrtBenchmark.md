## 性能测试

| Method                                           | x         |      Mean |     Error |    StdDev | Ratio | RatioSD | Code Size | Allocated | Alloc Ratio |
| ------------------------------------------------ | --------- | --------: | --------: | --------: | ----: | ------: | --------: | --------: | ----------: |
| ComputeScalarSseInverseSquareRootDivide          | 6.6695404 | 0.2321 ns | 0.0070 ns | 0.0058 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathReciprocalSqrtEstimate                       | 17.414614 | 8.2796 ns | 0.0560 ns | 0.0524 ns |     ? |       ? |      28 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathSqrt                                         | 20.733456 | 7.5462 ns | 0.0774 ns | 0.0686 ns |  1.00 |    0.01 |      28 B |         - |          NA |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootDirect          | 28.603512 | 0.2304 ns | 0.0070 ns | 0.0059 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathSqrt                                         | 30.997772 | 7.5516 ns | 0.0731 ns | 0.0684 ns |  1.00 |    0.01 |      28 B |         - |          NA |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootDivide          | 33.601086 | 0.2253 ns | 0.0069 ns | 0.0057 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathSqrt                                         | 40.171375 | 7.5255 ns | 0.0500 ns | 0.0444 ns |  1.00 |    0.01 |      28 B |         - |          NA |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootDirect          | 44.64195  | 1.2679 ns | 0.0152 ns | 0.0135 ns |     ? |       ? |      23 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootDirect          | 44.867973 | 1.2860 ns | 0.0185 ns | 0.0155 ns |     ? |       ? |      23 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootDirect          | 74.557915 | 0.2249 ns | 0.0064 ns | 0.0053 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarFastInverseSquareRoot               | 83.68503  | 0.2387 ns | 0.0114 ns | 0.0106 ns |     ? |       ? |      53 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarFastInverseSquareRoot               | 85.821014 | 0.1406 ns | 0.0085 ns | 0.0080 ns |     ? |       ? |      53 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonQuake     | 93.548164 | 0.3609 ns | 0.0097 ns | 0.0086 ns |     ? |       ? |      45 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootDivide          | 95.55012  | 0.2345 ns | 0.0110 ns | 0.0097 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonOptimized | 99.86023  | 0.2545 ns | 0.0080 ns | 0.0071 ns |     ? |       ? |      45 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonOptimized | 100.15018 | 0.2501 ns | 0.0086 ns | 0.0076 ns |     ? |       ? |      45 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathReciprocalSqrtEstimate                       | 131.23256 | 8.2829 ns | 0.0548 ns | 0.0486 ns |     ? |       ? |      28 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonOptimized | 131.58133 | 0.2547 ns | 0.0095 ns | 0.0084 ns |     ? |       ? |      45 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootDirect          | 174.36942 | 0.2300 ns | 0.0093 ns | 0.0082 ns |     ? |       ? |      20 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonQuake     | 208.06815 | 0.1482 ns | 0.0074 ns | 0.0069 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonOptimized | 228.72487 | 0.2380 ns | 0.0098 ns | 0.0082 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonQuake     | 287.032   | 0.2331 ns | 0.0089 ns | 0.0079 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonOptimized | 391.02368 | 0.2367 ns | 0.0049 ns | 0.0044 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonQuake     | 392.99518 | 0.2758 ns | 0.0088 ns | 0.0078 ns |     ? |       ? |      45 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| MathReciprocalSqrtEstimate                       | 405.40402 | 8.3135 ns | 0.0360 ns | 0.0319 ns |     ? |       ? |      28 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonQuake     | 428.7303  | 0.2359 ns | 0.0084 ns | 0.0074 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarFastInverseSquareRoot               | 447.21503 | 0.1336 ns | 0.0060 ns | 0.0053 ns |     ? |       ? |      53 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootDirect          | 522.1606  | 1.2742 ns | 0.0172 ns | 0.0161 ns |     ? |       ? |      23 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarSseInverseSquareRootNewtonOptimized | 554.3983  | 0.2357 ns | 0.0054 ns | 0.0045 ns |     ? |       ? |      44 B |         - |           ? |
|                                                  |           |           |           |           |       |         |           |           |             |
| ComputeScalarAvxInverseSquareRootNewtonQuake     | 725.4514  | 0.2770 ns | 0.0055 ns | 0.0049 ns |     ? |       ? |      45 B |         - |           ? |

## 汇编

[ScalarSqrtBenchmark-asm.md](../bin/Release/net8.0/BenchmarkDotNet.Artifacts/results/CSharp.Benchmark.Math.ScalarSqrtBenchmark-asm.md)