namespace PersonalAccount.Api.Models;

/// <summary>
/// Настройки Web API приложения.
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// Строка подключения MS SQL
    /// </summary>
    public required string MsSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Строка подключения PostgreSQL
    /// </summary>
    public required string PostgreSqlConnectionString { get; set; } = string.Empty;
}