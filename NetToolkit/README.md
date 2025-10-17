# NetToolkit
> 反正没有人用，自己摸索

基于 .net8 提供常用的工具

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
