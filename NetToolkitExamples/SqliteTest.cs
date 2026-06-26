using BenchmarkDotNet.Attributes;
using Dapper;
using LoveYuri.Core.Sqlite;

namespace NetToolkitExamples;

[MemoryDiagnoser, MarkdownExporter]
public class CacheComparisonBenchmark
{
    private const int HvValue = 1000;
    private const int MissingHvValue = -1;

    private int totalCount;
    private int hvCount;

    [GlobalSetup]
    public void GlobalSetup()
    {
        using var connection = SqliteService.CreateConnection(FullEoData.DatabaseName);
        totalCount = connection.ExecuteScalar<int>($"select count(*) from {FullEoData.TableName}");
        hvCount = connection.ExecuteScalar<int>(
            $"select count(*) from {FullEoData.TableName} where Hv = @Hv",
            new { Hv = HvValue });

        ValidateBuildSql();
        ValidateSelectAndCount();
        ValidateRepeatedParameterNames();
        ValidateUpdateSql();
    }

    [Benchmark]
    public string BuildSingleConditionSql()
    {
        return QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .BuildSql();
    }

    [Benchmark]
    public string BuildRepeatedConditionSql()
    {
        return QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, HvValue)
            .BuildSql();
    }

    [Benchmark]
    public int CountAll()
    {
        return QueryWrapper<FullEoData>.Query.Count();
    }

    [Benchmark]
    public int CountByHv()
    {
        return QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Count();
    }

    [Benchmark]
    public int SelectByHv()
    {
        return QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Select()
            .Count;
    }

    [Benchmark]
    public int SelectMissingByRepeatedHv()
    {
        return QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, MissingHvValue)
            .Select()
            .Count;
    }

    private static void ValidateBuildSql()
    {
        var sql = QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .OrderByDesc(k => k.ApIndex)
            .Limit(10, 20)
            .BuildSql();

        const string expected = "where ((Hv = @Hv)) ORDER BY ApIndex DESC limit 10 offset 20 ";
        if (sql != expected) {
            throw new InvalidOperationException($"BuildSql校验失败。Expected: {expected}; Actual: {sql}");
        }
    }

    private void ValidateSelectAndCount()
    {
        var wrapperCountAll = QueryWrapper<FullEoData>.Query.Count();
        if (wrapperCountAll != totalCount) {
            throw new InvalidOperationException($"CountAll校验失败。Expected: {totalCount}; Actual: {wrapperCountAll}");
        }

        var wrapperCount = QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Count();
        if (wrapperCount != hvCount) {
            throw new InvalidOperationException($"CountByHv校验失败。Expected: {hvCount}; Actual: {wrapperCount}");
        }

        var selectedCount = QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Select()
            .Count;
        if (selectedCount != hvCount) {
            throw new InvalidOperationException($"SelectByHv校验失败。Expected: {hvCount}; Actual: {selectedCount}");
        }
    }

    private static void ValidateRepeatedParameterNames()
    {
        var sql = QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, MissingHvValue)
            .BuildSql();

        const string expected = "where ((Hv = @Hv) AND (Hv = @Hv_1))";
        if (sql != expected) {
            throw new InvalidOperationException($"重复参数名校验失败。Expected: {expected}; Actual: {sql}");
        }

        var rows = QueryWrapper<FullEoData>.Query
            .Eq(k => k.Hv, HvValue)
            .Eq(k => k.Hv, MissingHvValue)
            .Select();
        if (rows.Count != 0) {
            throw new InvalidOperationException($"重复参数查询校验失败。Expected: 0; Actual: {rows.Count}");
        }
    }

    private static void ValidateUpdateSql()
    {
        var sql = QueryWrapper<FullEoData>.UpdateQuery
            .Set(k => k.Hv, HvValue)
            .Eq(k => k.ApIndex, 1)
            .BuildSql(BuildSqlType.Update);

        const string expected = "Hv = @Hv where ((ApIndex = @ApIndex))";
        if (sql != expected) {
            throw new InvalidOperationException($"UpdateSql校验失败。Expected: {expected}; Actual: {sql}");
        }
    }
}
