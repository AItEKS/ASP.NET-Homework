using Microsoft.Data.SqlClient;
using PersonalAccount.Domain.Dto;
using System.Reflection;

public class JournalProvider
{
    private readonly string _connectionString;

    public JournalProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Загрузка транзакций за период
    /// </summary>
    public List<JournalEntryDto> GetTransactions(DateTime startDate, DateTime endDate)
    {
        var result = new List<JournalEntryDto>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string sql = "select * from [dbo].[journal] where dater >= @start and dater <= @end";

            using (var command = new SqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@start", startDate);
                command.Parameters.AddWithValue("@end", endDate);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dto = MapObject<JournalEntryDto>(reader);
                        result.Add(dto);
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Универсальный метод маппинга через Reflection
    /// </summary>
    private T MapObject<T>(SqlDataReader reader) where T : new()
    {
        T obj = new T();
        Type type = typeof(T);

        // [CheckNumber, Amount]
        PropertyInfo[] properties = type.GetProperties();

        foreach (var prop in properties)
        {
            var attr = prop.GetCustomAttribute<DbColumnAttribute>();

            if (attr != null)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(attr.ColumnName);

                    if (!reader.IsDBNull(ordinal))
                    {
                        object val = reader.GetValue(ordinal);

                        Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        object safeValue;

                        if (targetType == typeof(DateTimeOffset) && val is DateTime dt)
                        {
                            safeValue = new DateTimeOffset(dt);
                        } else {
                            safeValue = Convert.ChangeType(val, targetType);
                        }
                        
                        prop.SetValue(obj, safeValue);
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    continue;
                }
            }
        }

        return obj;
    }
}