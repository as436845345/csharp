# InverseBenchmark

本基准测试用于评估 `1 / x`（倒数计算）在不同实现方式下的性能表现，涵盖**标量单一处理**与**数组批量处理**两种场景。

## 🔹 Scalar（标量单一处理）

该类别包含两个对比方法，完整实现详见：[InverseBenchmark.cs](../Math/InverseBenchmark.cs)

| 方法名 | 实现方式 | 说明 |
|--------|---------|------|
| `Reciprocal_Naive` | `1f / x` | 原生除法，作为性能基准（`[Baseline = true]`） |
| `Reciprocal_SseNewtonRaphson` | SSE `RCPSS` + 一次牛顿 - 拉夫逊迭代 | 利用 SIMD 指令加速，理论精度约 22 位 |

📊 **测试结论**：  
在单值计算场景下，`Reciprocal_Naive`（原生除法）性能最优。  
原因在于：现代 CPU 的标量除法单元已高度优化，而 SSE 的 `RCPSS` 指令虽快但精度有限，叠加牛顿迭代后反而引入额外指令开销，在单元素场景下无法体现优势。

📎 **参考资料**：
- 性能报告：[InverseBenchmark-report-github.md](../bin/Release/net8.0/BenchmarkDotNet.Artifacts/results/CSharp.Benchmark.Math.InverseBenchmark-report-github.md)
- 汇编代码分析：[InverseBenchmark-asm.md](../bin/Release/net8.0/BenchmarkDotNet.Artifacts/results/CSharp.Benchmark.Math.InverseBenchmark-asm.md)

---

> 💡 **延伸建议**：若需发挥 SIMD 优势，建议关注下文 **Vectorized（批量处理）** 场景——当数据量较大时，向量化实现的吞吐量优势将显著体现。