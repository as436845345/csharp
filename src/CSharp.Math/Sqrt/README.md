# ScalarSqrt.FastInverseSquareRoot

使用 Quake III Fast Inverse Square Root 方法。

## 外部链接

[Quake III&#39;s Fast Inverse Square Root Algorithm](https://mayukh-chr.github.io/posts/fast-inverse-square-root-algorithm/)[Quake III's Fast Inverse Square Root Algorithm.](https://mayukh-chr.github.io/posts/fast-inverse-square-root-algorithm/)<br/>
[Fast Inverse Square Root - miyasaka - 博客园](https://www.cnblogs.com/kion/p/17435423.html)

# Sse

- Sse.ReciprocalSqrtScalar

该指令直接计算 $1/\sqrt{x}$ 的近似值，速度极快（通常只需 1-2 个 CPU 周期），但精度较低（约 12 位有效数字）。

> 为了弥补精度不足，代码手动实现了一次牛顿迭代。