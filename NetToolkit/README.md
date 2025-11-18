# NetToolkit
> 反正没有人用，自己摸索

基于 .net8 提供常用的工具

### 1.0.4
1. 升级日志库版本为1.0.3
2. 拆分UpdateQueryWrapper和QueryWrapper,并修改Query为NewQuery和NewUpdate
3. 修改sqlite的特性到特性目录
4. 新增sqlite的[README文档](./Doc/sqlite.md)
5. sqlite: 移除list的insert方法，仅保留insertBatch方法
6. sqlite: 新增replace方法
7. sqlite: 优化实现

## 更新日志
### 1.0.3
1. 修改DiService类为ServiceContainer
2. 升级日志库版本为1.0.2

## 更新日志
### 1.0.2
1. 移除日志相关部分到love-yuri.Logger包中

### 1.0.1
1. 移除JetBrains依赖并更新程序集名称
2. 修改sqlite工具到Sqlite目录

### 1.0.0
1. 正式发布1.0正式版，大部分功能迁移自WpfCommon
2. 支持字符串格式化成常用数据类型: ToDouble, TryToDouble ...
3. 快速创建定时器 200.TimeOut(...); 200.TimeInterval(...);
4. 一键获取枚举的类型描述
5. Debouncer 防抖支持
6. Log.Info, Log.Error等高性能日志功能，支持控制台/文件单独或者同时输出
7. 提供UdpService类，支持快速发送/接收消息，以及优雅的开启、关闭、重联
8. 只用一个实体类即可快速操作sqlite数据，支持异步同步等不同方式
