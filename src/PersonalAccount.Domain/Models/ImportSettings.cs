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
    /// Описание настройки
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Уникальный код транзакции для начала загрузки.
    /// </summary>
    public required long StartPosition { get; set; }

    /// <summary>
    /// Размер пакета (батча) для вставки
    /// </summary>
    [BatchSize]
    public int BatchSize { get; set; } = 100;
}