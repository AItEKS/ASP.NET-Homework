using System;

namespace PersonalAccount.Console.Models;

public class ApplicationOptions
{   
    /// <summary>
    /// строка подключения MS SQL
    /// </summary>
    public required string ConnectionString { get; set; } = string.Empty;
}