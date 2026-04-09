namespace PersonalAccount.Console.Models;

/// <summary>
/// Настройки консольного приложения.
/// </summary>
public class ConsoleOptions
{
    /// <summary>
    /// URL нашего Web API
    /// </summary>
    public string ApiUrl { get; set; } = "http://localhost:8000/";

    /// <summary>
    /// ID организации, от имени которой работает эта касса
    /// </summary>
    public Guid CompanyId { get; set; }
    
    /// <summary>
    /// Строка подключения MS SQL
    /// </summary>
    public required string MsSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Строка подключения PostgreSQL
    /// </summary>
    public required string PostgreSqlConnectionString { get; set; } = string.Empty;
}