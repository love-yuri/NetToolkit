namespace LoveYuri.Core.Sqlite.Attribute;

[AttributeUsage(AttributeTargets.Class)]
public class TableInfoAttribute(string dataSource, string tableName) : System.Attribute {
    public string DataSource { get; } = dataSource;
    public string TableName { get; } = tableName;
}
