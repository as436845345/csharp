# Quake III Fast Inverse Square Root

## 性能测试

### 与 `System.Math.Sqrt` 比较

TODO: 性能测试表格

#### 汇编

- `System.Math.Sqrt`

```assembly
; CSharp.Benchmark.Math.SqrtBench.MathSqrt(Single)
       7FFE0E3B7CB0 vzeroupper
       7FFE0E3B7CB3 vcvtss2sd xmm0,xmm0,xmm1
       7FFE0E3B7CB7 vsqrtsd   xmm0,xmm0,xmm0
       7FFE0E3B7CBB vcvtsd2ss xmm0,xmm0,xmm0
       7FFE0E3B7CBF ret
; Total bytes of code 16
```

- `CSharp.Math.Sqrt.Sqrt.FastInverseSquareRoot`

```assembly
; CSharp.Benchmark.Math.SqrtBench.FastInverseSquareRoot(Single)
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