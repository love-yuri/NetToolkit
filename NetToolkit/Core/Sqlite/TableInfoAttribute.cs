namespace LoveYuri.Core.Sqlite;

[AttributeUsage(AttributeTargets.Class)]
public class TableInfoAttribute(string dataSource, string tableName) : Attribute {
    public string DataSource { get; } = dataSource;
    public string TableName { get; } = tableName;
}
