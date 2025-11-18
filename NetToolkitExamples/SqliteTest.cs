using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using LoveYuri.Core.Sqlite;

namespace NetToolkitExamples;

[MemoryDiagnoser, MarkdownExporter]
public class CacheComparisonBenchmark
{
    private static readonly MemberInfo _member = typeof(FullEoData).GetProperty("ApIndex");
    private static readonly ConcurrentDictionary<MemberInfo, string> _concurrentDict = new();
    private static readonly ConditionalWeakTable<MemberInfo, string> _weakTable = new();

    private Expression<Func<FullEoData, int>> expression = k => k.Hv;

    // [Benchmark]
    // public string ConcurrentDictionaryLookup()
    // {
    //     // 需要：哈希计算 + 锁竞争 + 内存屏障
    //     return _concurrentDict.GetOrAdd(expression.b_member, m => m.Name);
    // }

    [Benchmark]
    public async Task SelectAll()
    {
        await QueryWrapper<FullEoData>.NewQuery
            .SelectAsync();
    }

    [Benchmark]
    public async Task EqSelect ()
    {
        await QueryWrapper<FullEoData>.NewQuery
            .Eq(k => k.Hv, 1000)
            .Eq(k => k.Hv, 1000)
            .Eq(k => k.Hv, 1000)
            .Eq(k => k.Hv, 1000)
            .SelectAsync();
    }

    [Benchmark]
    public async Task EqSelect2 ()
    {
        await QueryWrapper<FullEoData>.NewQuery
            .SelectAsync();
    }

    // [Benchmark]
    // public string DirectAccessNoCache()
    // {
    //     return _member.Name;  // 纯反射开销
    // }
    //


}
