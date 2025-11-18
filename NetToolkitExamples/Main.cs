using System.Diagnostics;
using BenchmarkDotNet.Running;
using LoveYuri.Core.Sqlite;
using LoveYuri.Utils;
using NetToolkitExamples;
using Serilog;
using Log = LoveYuri.Utils.Log;

Serilog.Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()    // 输出到控制台
    .CreateLogger();

// BenchmarkRunner.Run<LogTest>();
// return 0;

var stopWatch = Stopwatch.StartNew();
for (var i = 0; i < 10000; i++) {
    Log.Info("Hello, World!");
}
stopWatch.Stop();

var stopWatchSer = Stopwatch.StartNew();
for (var i = 0; i < 10000; i++) {
    Serilog.Log.Information("Hello, World!");
}
stopWatchSer.Stop();

Log.Info($"yuri_log: 打印 10000条日志共耗时: {stopWatch.ElapsedMilliseconds}ms");
Log.Info($"Serilog: 打印 10000条日志共耗时: {stopWatchSer.ElapsedMilliseconds}ms");
