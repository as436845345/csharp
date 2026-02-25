## 性能测试

| Method                | x            | Mean      | Error     | StdDev    | Ratio | Code Size | Allocated | Alloc Ratio |
|---------------------- |------------- |----------:|----------:|----------:|------:|----------:|----------:|------------:|
| MathSqrt              | 2.436223E+37 | 5.0470 ns | 0.0409 ns | 0.0382 ns |  1.00 |      16 B |         - |          NA |
| FastInverseSquareRoot | 2.436223E+37 | 0.2372 ns | 0.0071 ns | 0.0063 ns |  0.05 |      53 B |         - |          NA |
| Sse_1                 | 2.436223E+37 | 0.2322 ns | 0.0063 ns | 0.0053 ns |  0.05 |      42 B |         - |          NA |
| Sse_2                 | 2.436223E+37 | 0.2339 ns | 0.0088 ns | 0.0083 ns |  0.05 |      42 B |         - |          NA |
| Sse_3                 | 2.436223E+37 | 0.2299 ns | 0.0070 ns | 0.0065 ns |  0.05 |      20 B |         - |          NA |
| Sse_4                 | 2.436223E+37 | 0.2332 ns | 0.0062 ns | 0.0055 ns |  0.05 |      20 B |         - |          NA |
| Avx_1                 | 2.436223E+37 | 0.2787 ns | 0.0216 ns | 0.0191 ns |  0.06 |      45 B |         - |          NA |
| Avx_2                 | 2.436223E+37 | 0.2472 ns | 0.0130 ns | 0.0116 ns |  0.05 |      45 B |         - |          NA |
| Avx_3                 | 2.436223E+37 | 1.2753 ns | 0.0171 ns | 0.0151 ns |  0.25 |      23 B |         - |          NA |

## 汇编

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.MathSqrt(Single)
       7FFEAC237CB0 vzeroupper
       7FFEAC237CB3 vcvtss2sd xmm0,xmm0,xmm1
       7FFEAC237CB7 vsqrtsd   xmm0,xmm0,xmm0
       7FFEAC237CBB vcvtsd2ss xmm0,xmm0,xmm0
       7FFEAC237CBF ret
; Total bytes of code 16
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.FastInverseSquareRoot(Single)
       7FFEAC207DE0 vzeroupper
       7FFEAC207DE3 vmovd     eax,xmm1
       7FFEAC207DE7 sar       eax,1
       7FFEAC207DE9 neg       eax
       7FFEAC207DEB add       eax,5F3759DF
       7FFEAC207DF0 vmovd     xmm0,eax
       7FFEAC207DF4 vmulss    xmm1,xmm1,dword ptr [7FFEAC207E18]
       7FFEAC207DFC vmulss    xmm1,xmm1,xmm0
       7FFEAC207E00 vmulss    xmm1,xmm1,xmm0
       7FFEAC207E04 vmovss    xmm2,dword ptr [7FFEAC207E1C]
       7FFEAC207E0C vsubss    xmm1,xmm2,xmm1
       7FFEAC207E10 vmulss    xmm0,xmm1,xmm0
       7FFEAC207E14 ret
; Total bytes of code 53
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Sse_1(Single)
       7FFEAC237E60 vzeroupper
       7FFEAC237E63 vrsqrtss  xmm0,xmm1,xmm1
       7FFEAC237E67 vmulps    xmm2,xmm0,xmm0
       7FFEAC237E6B vmulps    xmm1,xmm1,xmm2
       7FFEAC237E6F vmulps    xmm1,xmm1,dword bcst [7FFEAC237E90]
       7FFEAC237E79 vmovups   xmm2,[7FFEAC237EA0]
       7FFEAC237E81 vsubps    xmm1,xmm2,xmm1
       7FFEAC237E85 vmulps    xmm0,xmm0,xmm1
       7FFEAC237E89 ret
; Total bytes of code 42
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Sse_2(Single)
       7FFEAC207E20 vzeroupper
       7FFEAC207E23 vrsqrtss  xmm0,xmm1,xmm1
       7FFEAC207E27 vmulps    xmm2,xmm0,xmm0
       7FFEAC207E2B vmulps    xmm1,xmm1,xmm2
       7FFEAC207E2F vmovups   xmm2,[7FFEAC207E50]
       7FFEAC207E37 vsubps    xmm1,xmm2,xmm1
       7FFEAC207E3B vmulps    xmm0,xmm0,xmm1
       7FFEAC207E3F vmulps    xmm0,xmm0,dword bcst [7FFEAC207E60]
       7FFEAC207E49 ret
; Total bytes of code 42
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Sse_3(Single)
       7FFEAC227D30 vzeroupper
       7FFEAC227D33 vsqrtss   xmm0,xmm1,xmm1
       7FFEAC227D37 vmovss    xmm1,dword ptr [7FFEAC227D48]
       7FFEAC227D3F vdivss    xmm0,xmm1,xmm0
       7FFEAC227D43 ret
; Total bytes of code 20
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Sse_4(Single)
       7FFEAC237D70 vzeroupper
       7FFEAC237D73 vsqrtss   xmm0,xmm1,xmm1
       7FFEAC237D77 vmovups   xmm1,[7FFEAC237D90]
       7FFEAC237D7F vdivss    xmm0,xmm1,xmm0
       7FFEAC237D83 ret
; Total bytes of code 20
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Avx_1(Single)
       7FFEAC1F7EF0 vzeroupper
       7FFEAC1F7EF3 vrsqrtps  ymm0,ymm1
       7FFEAC1F7EF7 vmulps    ymm2,ymm0,ymm0
       7FFEAC1F7EFB vmulps    ymm1,ymm1,ymm2
       7FFEAC1F7EFF vmulps    ymm1,ymm1,dword bcst [7FFEAC1F7F20]
       7FFEAC1F7F09 vmovups   ymm2,[7FFEAC1F7F40]
       7FFEAC1F7F11 vsubps    ymm1,ymm2,ymm1
       7FFEAC1F7F15 vmulps    ymm0,ymm0,ymm1
       7FFEAC1F7F19 vzeroupper
       7FFEAC1F7F1C ret
; Total bytes of code 45
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Avx_2(Single)
       7FFEAC217E70 vzeroupper
       7FFEAC217E73 vrsqrtps  ymm0,ymm1
       7FFEAC217E77 vmulps    ymm2,ymm0,ymm0
       7FFEAC217E7B vmulps    ymm1,ymm1,ymm2
       7FFEAC217E7F vmovups   ymm2,[7FFEAC217EA0]
       7FFEAC217E87 vsubps    ymm1,ymm2,ymm1
       7FFEAC217E8B vmulps    ymm0,ymm0,ymm1
       7FFEAC217E8F vmulps    ymm0,ymm0,dword bcst [7FFEAC217EC0]
       7FFEAC217E99 vzeroupper
       7FFEAC217E9C ret
; Total bytes of code 45
```

## .NET 8.0.24 (8.0.24, 8.0.2426.7010), X64 RyuJIT x86-64-v4 (Job: .NET 8.0(Runtime=.NET 8.0))

```assembly
; CSharp.Benchmark.Math.SqrtBenchmark.Avx_3(Single)
       7FFEAC207D30 vzeroupper
       7FFEAC207D33 vsqrtps   ymm0,ymm1
       7FFEAC207D37 vmovss    xmm1,dword ptr [7FFEAC207D48]
       7FFEAC207D3F vdivss    xmm0,xmm1,xmm0
       7FFEAC207D43 vzeroupper
       7FFEAC207D46 ret
; Total bytes of code 23
```