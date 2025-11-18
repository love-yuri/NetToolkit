# 基于grpc通信的sqlite库

## 基础库

1. sqlite3
2. .net8
3. grpc

## 使用

### QueryWrapper

> 假设有个数据库 `ConfigDb.s3db` 里面有表`SysComponents` 字段如下
>
> ```sqlite
> create table SysComponents
> (
>         ComID       INTEGER     not null primary key,
>         Enabled     BOOLEAN     not null,
>         ComName     VARCHAR(50) not null unique,
>         DisplayName VARCHAR(50),
>         ComTypeName VARCHAR(50),
>         Description NVARCHAR(200)
> );
> ```

1. 定义实体类proto 文件, 命名空间需要对应数据库名,proto message名需要是表名.

   ```protobuf
   syntax = "proto3";
   
   option csharp_namespace = "Proto.Database.ConfigDb";
   package Proto.Database.ConfigDb;
   
   message SysComponents {
     int32 ComID = 1;
     bool Enabled = 2;
     string ComName = 3;
     string DisplayName = 4;
     string ComTypeName = 5;
     optional string Description = 6;
   }
   ```

2. 将该proto文件加入c#工程: `<Protobuf Include="Protos\database\config-db.proto" />` 

3. 然后就可以通过生成的实体类进行查询了

4. 实体类通过依赖`QueryWrapper` 类完成，他支持链式查询。 所有生成的实体类都可以通过该类构造查询条件。IMessage 为proto的接口，因为需要通过grpc传输所以必须继承自该接口。

5. 类定义 `public class QueryWrapper<T> where T : IMessage, new()` 

### 查询

#### 条件查询

> 通过链式调用构造一个完整的条件查询，构造过程以此往下。以下只是一个简单的示例。
>
> 它可以生成以下where语句: `select * from SysComponents where ComID not in (1, 2, 3, 4) and ComId = 33`
>
> ```c#
> QueryWrapper<SysComponents>.Builder()
>         .NotIn(k => k.ComID, [1, 2, 3, 4])
>         .Eq(k => k.ComID, 33)
> ```

以下提供几种 基础方法. tips: 使用前需要`QueryWrapper<SysComponents>.Builder()` 进行构造.

- `Eq`查找相等数据；使用示例: `queryWrapper.Eq(k => k.ComID, 1);` 查询ComId为1的数据。
- `Neq` 查找不等数据；使用示例: `queryWrapper.Neq(k => k.ComID, 1);` 查询ComId不为1的数据。
- `Gt` 使用示例: `queryWrapper.v(k => k.ComID, 1);` 查询ComId大于1的数据。
- `Gte` 使用示例: `queryWrapper.Neq(k => k.ComID, 1);` 查询ComId大于等于1的数据。
- `Lt`使用示例: `queryWrapper.Lt(k => k.ComID, 1);` 查询ComId小于1的数据。
- `Lte`使用示例: `queryWrapper.Lte(k => k.ComID, 1);` 查询ComId小于等于的数据。
- `Like` 使用示例:`queryWrapper.Like(k => k.DisplayName, "%Smart%");` 查询DisplayName相似Smart。
- `In`  查询指定范围内数据。使用示例: `queryWrapper.In(k => k.ComID, [1, 2, 3, 4]);` 查询ComId在以下范围内的数据.
- `NotIn` 查询不在指定范围内数据使用示例: `queryWrapper.NotIn(k => k.ComID, [1, 2, 3, 4]);` 查询ComId不在以下范围内的数据.
- `Between` 查询指定范围内数据。 使用示例: `queryWrapper.Between(k => k.ComID, 1, 10);` 查找ComID在1-10里的数据。
- `NotBetween` 查询指定范围内数据。 使用示例: `queryWrapper.NotBetween(k => k.ComID, 1, 10);` 查找ComID不在1-10里的数据。
- `IsNull` 查找为null的数据，使用示例: `queryWrapper.IsNull(k => k.Description)` 查找Description是null的数据。
- `IsNotNull` 查找不为null的数据，使用示例: `queryWrapper.IsNotNull(k => k.Description)` 查找Description不是null的数据。

#### 排序和分页

> 排序扩展同样来自于QueryWrapper和查询条件一样用就行。
>
> tips: 构建顺序决定排序顺序，第一顺位相同则会判断第二顺位，这是sql的排序机制。

```c#
var queryWrapper = QueryWrapper<SysComponents>.Builder()
    .xxx // 这里可以添加别的查询条件
    .OrderByDesc(p => p.ComID) // 按comId逆序排序
    .OrderBy(p => p.Enabled) // 按Enable正序排序
    .OrderBy(p => p.ComName) // 按 comName正序排序
    .Limit(10)        // limit可多次调用，但以最后一次调用为主
	.Limit(10, 20); // 查找10个，偏移20个，偏移量默认可以省略
```

#### Or

> 所有连接默认是And，如果需要or，需要在操作函数第三个参数注明连接符号

```c#
var queryWrapper = QueryWrapper<SysComponents>.Builder()
    .Eq(p => p.ComID, 1)
    .Eq(p => p.ComID, 2, LogicalOperatorType.Or); // 显式表示和前一个符号连接为or

// 生成sql: where ComId = 1 or ComdId = 2
```

#### group

> 所有调用都不是一组的，也就是说如果你需要一组操作需要使用Group

```c#
var queryWrapper = QueryWrapper<SysComponents>.Builder()
    .Eq(p => p.ComID, 1)
    .Group(w => {
        w.In(p => p.ComID, [20, 30, 40]);
        w.Neq(p => p.ComName, "test");
    }, LogicalOperatorType.Or);

// 生成sql where ComdId = 1 or ((ComId in (20, 30, 40)) and (ComName <> 'test')) 
```


### 插入

```c#
var dataList = new List<SysComponents>();
int res = dataList.Insert(client); // 通过扩展方法调用插入list 数据

// 插入单个数据
int res = new SysComponents {
    ComID = 1
}.Insert(client);

// 手动调用grpc
var dataList = new List<SysComponents>();
var tableInfo = SqliteCore.GetTableInfo<SysComponents>();
var request = new InsertRequest {
    TableInfo = tableInfo,
};
request.Data.AddRange(dataList.Select(k => Any.Pack(k)));
var ret = client.Insert(request);
```

### 更新

> 更新操作和查询操作一致，只是多了个Set 而已，如果不加where条件将会更新所有数据

```c#
int res = UpdateQueryWrapper<SysComponents>.BuilderUpdate()
    .Set(p => p.DisplayName, "yuri is yes")
    .Set(p => p.Description, "yuri is yes22")
    .In(p => p.ComID, [21, 10, 31, 32, 33, 44])
    .Update(client);
Console.WriteLine($"更新res: {res}");
```

### 删除

> 和查询方法保持一致，最后调用Delete方法

```c#
int res = QueryWrapper<SysComponents>.Builder()
    .Between(k => k.ComID, 200, 400)
    .Delete(client);

Console.WriteLine($"delete: {res}");
```

### 调用grpc

> 以上只是构造了查询条件需要转换为grpc通信。核心方法: `queryWrapper.CreateQueryRequest();`他会构建出一个grpc查询通信request出来。然后通过client就可以直接调用该查询条件构建出来查询。可以用于`Select` `Delete` ，他们两个都是用的默认`QueryWrapper`。 Update使用他的子类`UpdateQueryWrapper` ，他仅仅多了update的Set方法，别的使用方式一致。

```c#
// 以下演示如何从头开始调用grpc的GetAll 查询。
var queryWrapper = QueryWrapper<SysComponents>.Builder()
    .OrderByDesc(p => p.ComID)
    .OrderBy(p => p.Enabled)
    .Limit(10, 20);
var request = queryWrapper.CreateQueryRequest();
var response = client.GetAll(request);
List<SysComponents> components = response.Data.Select(any => any.Unpack<SysComponents>()).ToList();

// 也可以使用封装好的方法
List<SysComponents> components = QueryWrapper<SysComponents>.Builder()
    .IsNotNull(k => k.Description)
    .OrderBy(p => p.ComName)
    .Limit(2, 5)
    .Select(client); // 直接调用扩展方法Select就行查询

// 异步方法
Task<List<SysComponents>> components = QueryWrapper<SysComponents>.Builder()
    .IsNotNull(k => k.Description)
    .OrderBy(p => p.ComName)
    .Limit(2, 5)
    .SelectAsync(client);
```

## 数据结构

### SqliteService

> 定义grpc端sqlite服务

```protobuf
// 服务定义
service SqliteService {
    rpc Select(QuerySqlRequest) returns (QuerySqlResponse);
    rpc Insert(InsertRequest) returns (ModifySqlResponse);
    rpc Delete(QuerySqlRequest) returns (ModifySqlResponse);
    rpc Update(QuerySqlRequest) returns (ModifySqlResponse);
}
```

### QuerySqlRequest

> 查询服务所有参数，默认不需要手动传递，使用`QueryWrapper` 自动生成就好。

```protobuf
// 查询服务统一request
message QuerySqlRequest {
    optional TableInfo table_info = 1; // 表信息
    optional string where_clause = 2; // where语句
    optional string limit_clause = 3; // limit语句
    optional string order_by_clause = 4; // order by 语句
    optional string set_clause = 5; // update语句
    map<string, ProtoObject> where_values = 6; // 参数列表
}
```

### ProtoObject

> 处理不同数据的展示

```protobuf
// 泛型object
message ProtoObject {
    oneof value {
        int64 int_value = 1;
        double double_value = 2;
        string string_value = 3;
        Collection collection_value = 4;
    }
    message Collection {
        repeated ProtoObject items = 1;
    }
}
```

### TableInfo

> 表信息，只要数据库名-表名就行

```protobuf
// 表信息
message TableInfo {
    optional string database = 1; // 数据库名
    optional string tableName = 2; // 表名
}
```

### QuerySqlResponse

> 查询操作返回值 返回实体类的集合

```protobuf
// 查询sql response
message QuerySqlResponse {
    repeated google.protobuf.Any data = 1;
}
```

### InsertRequest

> 插入操作只要携带TableInfo和数据就行。Data 直接压入实体类就好

```protobuf
// 插入请求
message InsertRequest {
    optional TableInfo table_info = 1;
    repeated google.protobuf.Any data = 3;  // 要插入的数据
}
```

### ModifySqlResponse

> 修改操作返回信息，返回影响的行数

```protobuf
// 修改操作响应
message ModifySqlResponse {
    optional int32 rows_affected = 3;    // 影响的行数
}
```

## 性能测试

### BenchmarkDotNet 报告参数详解

| 列名            | 说明               | 重要性 | 示例值        | 解读                                 |
| --------------- | ------------------ | ------ | ------------- | ------------------------------------ |
| **Method**      | 测试的方法名称     | -      | `DapperQuery` | 标识被测试的代码路径                 |
| **Mean**        | 平均执行时间       | ★★★★★  | `1.234 ms`    | 核心性能指标，值越小越好             |
| **StdDev**      | 标准差             | ★★★★   | `0.045 ms`    | 衡量数据波动性，>Mean的10%需注意     |
| **Ratio**       | 相对基准方法的比值 | ★★★★   | `1.50`        | 对比基准方法（如Baseline）的性能倍数 |
| **RatioSD**     | 比率的标准差       | ★★     | `0.12`        | Ratio的稳定性指标                    |
| **Gen0**        | 第0代GC回收次数    | ★★★    | `62.50`       | 短期对象内存压力，值高需优化         |
| **Gen1**        | 第1代GC回收次数    | ★★     | `1.25`        | 中期对象内存压力                     |
| **Gen2**        | 第2代GC回收次数    | ★      | `0.00`        | 长期对象/LOH内存压力                 |
| **Allocated**   | 内存分配总量       | ★★★★★  | `320 KB`      | 关键优化指标，越少越好               |
| **Alloc Ratio** | 内存分配比率       | ★★★    | `1.80`        | 对比基准方法的内存使用倍数           |

### 测试结果

> 测试数据集: ConfigDb.SysComponents
>
> 测试对比数据量: 20条 / 9930条

- GetaAllTest : 创建grpc通道-发送请求-接收请求-解析请求 完整过程。包含protobuf的数据序列化/反序列化网络传输
- NormalTest: 所有操作和GetaAllTest 保持一致，但是省略grpc连接-传输过程
- NormalTest2： 所有操作和NormalTest保持一致，但是省略序列化返回数据的过程
- NormalTest3： 直连sqlite 使用Dapper查询数据并解析到对应实体类
- PureAdoNetQuery： Microsoft.Data.Sqlite 直接查询，仅计数
- PureAdoNetWithMapping： Microsoft.Data.Sqlite 直接查询，并解析到对应的数据实体类

| Method                |        Mean |     Error |    StdDev | Ratio |      Gen0 |     Gen1 |     Gen2 |  Allocated |
| --------------------- | ----------: | --------: | --------: | ----: | --------: | -------: | -------: | ---------: |
| GetaAllTest           | 22,520.8 us | 448.75 us | 629.08 us | 26.53 |  500.0000 | 468.7500 | 156.2500 | 3133.57 KB |
| NormalTest            | 16,097.3 us | 310.90 us | 393.19 us | 18.96 | 1000.0000 | 968.7500 | 375.0000 | 6748.78 KB |
| NormalTest2           |  7,736.9 us |  99.20 us |  87.94 us |  9.11 |  367.1875 | 351.5625 | 164.0625 | 3013.95 KB |
| NormalTest3           |  6,984.4 us |  44.85 us |  41.95 us |  8.23 |  359.3750 | 351.5625 | 164.0625 | 2968.21 KB |
| PureAdoNetQuery       |    848.9 us |   3.54 us |   3.14 us |  1.00 |         - |        - |        - |    1.04 KB |
| PureAdoNetWithMapping |  5,978.8 us |   8.83 us |   7.82 us |  7.04 |  328.1250 | 289.0625 | 125.0000 | 2502.75 KB |

| Method                |        Mean |     Error |    StdDev | Ratio |      Gen0 |     Gen1 |     Gen2 |  Allocated |
| --------------------- | ----------: | --------: | --------: | ----: | --------: | -------: | -------: | ---------: |
| GetaAllTest           | 26,976.7 us | 527.54 us | 722.10 us | 31.52 | 1187.5000 | 687.5000 | 218.7500 | 8012.43 KB |
| NormalTest            | 16,003.0 us | 158.77 us | 132.58 us | 18.70 | 1000.0000 | 968.7500 | 375.0000 | 6748.95 KB |
| NormalTest2           |  8,116.1 us | 114.40 us | 101.42 us |  9.48 |  375.0000 | 343.7500 | 156.2500 | 3013.99 KB |
| NormalTest3           |  6,947.2 us |  35.59 us |  29.72 us |  8.12 |  359.3750 | 351.5625 | 164.0625 | 2968.23 KB |
| PureAdoNetQuery       |    855.9 us |   4.18 us |   3.70 us |  1.00 |         - |        - |        - |    1.04 KB |
| PureAdoNetWithMapping |  6,019.6 us |  91.44 us |  85.54 us |  7.03 |  328.1250 | 289.0625 | 125.0000 | 2502.74 KB |
