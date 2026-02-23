[AttributeUsage(AttributeTargets.Property)]
public class DbColumnAttribute : Attribute
{
    public string ColumnName { get; }
    public Type DataType { get; }

    public DbColumnAttribute(string columnName, Type dataType)
        {
            ColumnName = columnName;
            DataType = dataType;
        }
}