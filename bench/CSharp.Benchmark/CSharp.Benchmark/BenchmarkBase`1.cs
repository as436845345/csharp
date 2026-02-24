using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace CSharp.Benchmark;

[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[DisassemblyDiagnoser(printSource: true, printInstructionAddresses: true, exportCombinedDisassemblyReport: true, exportDiff: true)]
public abstract class BenchmarkBase<T>
    where T : class
{
    public static void ExecuteByBenchmarkRunner()
    {
        BenchmarkRunner.Run<T>();
    }
}
