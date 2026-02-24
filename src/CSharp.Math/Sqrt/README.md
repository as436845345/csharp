# Quake III Fast Inverse Square Root

## 性能测试

### 与 `System.Math.Sqrt` 比较

| Method                      | x   | Mean      | Error     | StdDev    | Ratio | Code Size | Allocated | Alloc Ratio |
|---------------------------- |---- |----------:|----------:|----------:|------:|----------:|----------:|------------:|
| MathSqrt_Bench              | 2   | 5.1189 ns | 0.0320 ns | 0.0283 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 2   | 0.2387 ns | 0.0183 ns | 0.0171 ns |  0.05 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 2.5 | 5.0682 ns | 0.0394 ns | 0.0329 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 2.5 | 0.2312 ns | 0.0107 ns | 0.0100 ns |  0.05 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 3   | 5.0950 ns | 0.0350 ns | 0.0327 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 3   | 0.2393 ns | 0.0069 ns | 0.0062 ns |  0.05 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 4   | 3.9161 ns | 0.0324 ns | 0.0303 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 4   | 0.2807 ns | 0.0298 ns | 0.0417 ns |  0.07 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 5   | 5.3190 ns | 0.0178 ns | 0.0148 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 5   | 0.2313 ns | 0.0247 ns | 0.0231 ns |  0.04 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 6   | 5.2418 ns | 0.0172 ns | 0.0153 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 6   | 0.2467 ns | 0.0069 ns | 0.0058 ns |  0.05 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 7   | 5.0613 ns | 0.0356 ns | 0.0333 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 7   | 0.2368 ns | 0.0073 ns | 0.0061 ns |  0.05 |      53 B |         - |          NA |
|                             |     |           |           |           |       |           |           |             |
| MathSqrt_Bench              | 10  | 5.0561 ns | 0.0289 ns | 0.0256 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot_Bench | 10  | 0.2350 ns | 0.0108 ns | 0.0090 ns |  0.05 |      53 B |         - |          NA |

#### 汇编

- `System.Math.Sqrt`

```assembly
; CSharp.Benchmark.Math.SqrtBench.MathSqrt_Bench(Single)
       7FFE0E3B7CB0 vzeroupper
       7FFE0E3B7CB3 vcvtss2sd xmm0,xmm0,xmm1
       7FFE0E3B7CB7 vsqrtsd   xmm0,xmm0,xmm0
       7FFE0E3B7CBB vcvtsd2ss xmm0,xmm0,xmm0
       7FFE0E3B7CBF ret
; Total bytes of code 16
```

- `CSharp.Math.Sqrt.Sqrt.FastInverseSquareRoot`

```assembly
; CSharp.Benchmark.Math.SqrtBench.FastInverseSquareRoot_Bench(Single)
       7FFE0E397DE0 vzeroupper
       7FFE0E397DE3 vmovd     eax,xmm1
       7FFE0E397DE7 sar       eax,1
       7FFE0E397DE9 neg       eax
       7FFE0E397DEB add       eax,5F3759DF
       7FFE0E397DF0 vmovd     xmm0,eax
       7FFE0E397DF4 vmulss    xmm1,xmm1,dword ptr [7FFE0E397E18]
       7FFE0E397DFC vmulss    xmm1,xmm1,xmm0
       7FFE0E397E00 vmulss    xmm1,xmm1,xmm0
       7FFE0E397E04 vmovss    xmm2,dword ptr [7FFE0E397E1C]
       7FFE0E397E0C vsubss    xmm1,xmm2,xmm1
       7FFE0E397E10 vmulss    xmm0,xmm1,xmm0
       7FFE0E397E14 ret
; Total bytes of code 53
```

## 外部链接

[Quake III&#39;s Fast Inverse Square Root Algorithm](https://mayukh-chr.github.io/posts/fast-inverse-square-root-algorithm/)[Quake III's Fast Inverse Square Root Algorithm.](https://mayukh-chr.github.io/posts/fast-inverse-square-root-algorithm/)<br/>
[Fast Inverse Square Root - miyasaka - 博客园](https://www.cnblogs.com/kion/p/17435423.html)