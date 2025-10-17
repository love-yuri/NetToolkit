using System.Diagnostics;
using LoveYuri.Utils;

// 测试参数
var testDurationMs = 1000; // 测试时长 1 秒
int threadCount = Environment.ProcessorCount; // 使用所有 CPU 核心

// 启动多线程测试
long writeCount = 0;
var stopwatch = Stopwatch.StartNew();

// Log.AppendLogToFile("log.txt");

Parallel.For(0, threadCount, _ => {
    while (stopwatch.ElapsedMilliseconds < testDurationMs) {
        Log.Info("测试日志内容");
        Interlocked.Increment(ref writeCount); // 线程安全计数
    }
});

stopwatch.Stop();

// 输出结果
Console.WriteLine($"1 秒内写入日志次数: {writeCount}");
Console.WriteLine($"平均每秒写入: {writeCount / (testDurationMs / 1000.0):N0} 条");
Console.ReadKey();
