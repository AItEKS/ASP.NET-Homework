using System;

namespace PersonalAccount.Api.Models;

/// <summary>
/// Настройки Web API приложения.
/// </summary>
public class ApiOptions
{
    /// <summary>
    /// Строка подключения PostgreSQL
    /// </summary>
    public required string PostgreSqlConnectionString { get; set; } = string.Empty;
}