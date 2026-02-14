using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Validation;

namespace PersonalAccount.Domain.Models;

public class ImportSettings
{
    /// <summary>
    /// ID настройки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Тип источника
    /// </summary>
    public required ImportSourceType SourceType { get; set; }

    /// <summary>
    /// Размер пакета (батча) для вставки
    /// </summary>
    [BatchSize]
    public int BatchSize { get; set; } = 100;

    /// <summary>
    /// Маппинг колонок (конфигурация в JSON)
    /// </summary>
    [ValidJson]
    public required string ColumnMapping { get; set; }
}