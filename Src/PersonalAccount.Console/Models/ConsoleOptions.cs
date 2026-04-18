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

    public Guid BranchId { get; set; }
    
    /// <summary>
    /// Строка подключения MS SQL
    /// </summary>
    public required string MsSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Размер батча
    /// </summary>
    public int BatchSize { get; set; } = 1000;
}