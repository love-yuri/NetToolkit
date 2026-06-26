# SQLite 快速工具

基于 `Microsoft.Data.Sqlite` 和 `Dapper` 的轻量 sqlite 操作封装。实体类通过 `TableInfoAttribute` 绑定数据库文件和表名，查询条件通过 `QueryWrapper<T>` 链式构造。

## 实体定义

```csharp
using LoveYuri.Core.Sqlite.Attribute;

[TableInfo(@"D:\data\app.s3db", "SysComponents")]
public class SysComponents
{
    public int ComID { get; set; }
    public bool Enabled { get; set; }
    public string ComName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
```

当前字段名默认使用属性名，实体的公开可读写属性会参与插入、替换和查询映射。

## 查询入口

推荐入口：

```csharp
QueryWrapper<SysComponents>.Query
QueryWrapper<SysComponents>.UpdateQuery
```


## 条件查询

```csharp
List<SysComponents> rows = QueryWrapper<SysComponents>.Query
    .NotIn(x => x.ComID, [1, 2, 3, 4])
    .Eq(x => x.Enabled, true)
    .OrderByDesc(x => x.ComID)
    .Limit(10)
    .Select();
```

常用条件方法：

| 方法 | 说明 |
| --- | --- |
| `Eq` | 等于 |
| `Neq` | 不等于 |
| `Gt` / `Gte` | 大于 / 大于等于 |
| `Lt` / `Lte` | 小于 / 小于等于 |
| `Like` | `LIKE` 匹配 |
| `In` / `NotIn` | 在集合内 / 不在集合内 |
| `Between` / `NotBetween` | 在区间内 / 不在区间内 |
| `IsNull` / `IsNotNull` | 空值判断 |

连接符默认是 `AND`。需要 `OR` 时，在条件方法的最后一个参数传入 `LogicalOperatorType.Or`：

```csharp
var rows = QueryWrapper<SysComponents>.Query
    .Eq(x => x.ComID, 1)
    .Eq(x => x.ComID, 2, LogicalOperatorType.Or)
    .Select();
```

## 分组、排序和分页

```csharp
var rows = QueryWrapper<SysComponents>.Query
    .Eq(x => x.Enabled, true)
    .Group(g => {
        g.Like(x => x.ComName, "%core%");
        g.IsNotNull(x => x.Description);
    }, LogicalOperatorType.Or)
    .OrderByDesc(x => x.ComID)
    .OrderBy(x => x.ComName)
    .Limit(10, 20)
    .Select();
```

## 查询执行

```csharp
List<SysComponents> rows = QueryWrapper<SysComponents>.Query.Select();
SysComponents? row = QueryWrapper<SysComponents>.Query.Eq(x => x.ComID, 1).SelectOne();
int count = QueryWrapper<SysComponents>.Query.Eq(x => x.Enabled, true).Count();

List<SysComponents> asyncRows = await QueryWrapper<SysComponents>.Query.SelectAsync();
SysComponents? asyncRow = await QueryWrapper<SysComponents>.Query.Eq(x => x.ComID, 1).SelectOneAsync();
int asyncCount = await QueryWrapper<SysComponents>.Query.Eq(x => x.Enabled, true).CountAsync();
```

## 更新

更新入口使用 `UpdateQuery`，最终执行动作仍然是 `.Update()` / `.UpdateAsync()`：

```csharp
int rows = QueryWrapper<SysComponents>.UpdateQuery
    .Set(x => x.DisplayName, "yuri is yes")
    .Set(x => x.Description, "updated")
    .In(x => x.ComID, [21, 10, 31])
    .Update();
```

如果不加 `where` 条件，将会更新整张表。

## 删除

```csharp
int rows = QueryWrapper<SysComponents>.Query
    .Between(x => x.ComID, 200, 400)
    .Delete();
```

如果不加 `where` 条件，将会删除整张表。

## 插入和替换

```csharp
var entity = new SysComponents {
    ComID = 1,
    Enabled = true,
    ComName = "Core"
};

int inserted = entity.Insert();
int replaced = entity.Replace();

int batchInserted = new[] { entity }.InsertBatch();
```

对应异步方法：

```csharp
await entity.InsertAsync();
await entity.ReplaceAsync();
await new[] { entity }.InsertBatchAsync();
```

## 直接执行 SQL

```csharp
var rows = SqliteService.Execute<SysComponents>(
    "select * from SysComponents where Enabled = @Enabled",
    new { Enabled = true });

var asyncRows = await SqliteService.ExecuteAsync<SysComponents>(
    "select * from SysComponents where Enabled = @Enabled",
    new { Enabled = true });
```
