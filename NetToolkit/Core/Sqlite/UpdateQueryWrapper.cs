using System.Linq.Expressions;
using System.Text;

namespace LoveYuri.Core.Sqlite;

/// <summary>
/// 泛型更新构造器，提供强类型的链式查询条件构建
/// </summary>
/// <typeparam name="T">实体类型，必须实现 IMessage 接口</typeparam>
public class UpdateQueryWrapper<T>: QueryWrapper<T> {

    /// <summary>
    /// 构建set sql
    /// </summary>
    private readonly StringBuilder setClauseBuilder = new();

    /// <summary>
    /// 更新构造器不提供查询入口，请从 QueryWrapper&lt;T&gt; 创建。
    /// </summary>
    [Obsolete("请使用 QueryWrapper<T>.Query。", true)]
    public new static QueryWrapper<T> Query => throw new NotSupportedException("请使用 QueryWrapper<T>.Query。");

    /// <summary>
    /// 更新构造器不提供更新入口，请从 QueryWrapper&lt;T&gt; 创建。
    /// </summary>
    [Obsolete("请使用 QueryWrapper<T>.UpdateQuery。", true)]
    public new static UpdateQueryWrapper<T> UpdateQuery => throw new NotSupportedException("请使用 QueryWrapper<T>.UpdateQuery。");

    /// <summary>
    /// 更新 某个字段
    /// </summary>
    /// <param name="expression">字段名</param>
    /// <param name="value">字段值</param>
    /// <example>
    /// <code>
    /// xxx.Set(p.ComId, 1)
    /// </code>
    /// 生成的SQL类似：set ComId = 1
    /// </example>
    public UpdateQueryWrapper<T> Set<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
    {
        string fieldName = GetFieldName(expression);
        string key = GenerateUniqueParamKey(fieldName);

        if (setClauseBuilder.Length != 0) {
            setClauseBuilder.Append(',');
        }

        setClauseBuilder.Append(fieldName).Append(" = ").Append(key);
        Values[key] = value!;

        return this;
    }

    public override string BuildSql(BuildSqlType type = BuildSqlType.Select)
    {
        var stringBuilder = new StringBuilder();
        if (setClauseBuilder.Length == 0) {
            throw new Exception("update语句必须要set!");
        }
        stringBuilder.Append(setClauseBuilder);
        stringBuilder.Append(' ');

        if (ConditionBuilder.Length > 0) {
            stringBuilder.Append("where (");
            stringBuilder.Append(ConditionBuilder);
            stringBuilder.Append(')');
        }
        return stringBuilder.ToString();
    }
}
