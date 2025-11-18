using BenchmarkDotNet.Attributes;
using Serilog;

namespace NetToolkitExamples;

[MemoryDiagnoser]
public class LogTest {

    [GlobalSetup]
    public void Init()
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()    // 输出到控制台
            .CreateLogger();
    }

    [Benchmark]
    public void YuriLog()
    {
        for (int i = 0; i < 5000; i++) {
            LoveYuri.Utils.Log.Info("yuri is yes");
        }
    }

    [Benchmark]
    public void SerilogLog()
    {
        for (int i = 0; i < 5000; i++) {
            Serilog.Log.Information("yuri is yes");
        }
    }
}
