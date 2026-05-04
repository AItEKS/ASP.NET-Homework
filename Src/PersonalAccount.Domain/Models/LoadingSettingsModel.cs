using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.Domain.Models;

/// <summary>
/// Модель настроек загрузки данных.
/// </summary>
public class LoadingSettingsModel : DomainModel
{
    /// <summary>
    /// ОФилиал-владелец настроек.
    /// </summary>
    [Required]
    public BranchModel Owner {get;set;} = null!;

    /// <summary>
    /// Описание настройки
    /// </summary>
    [StringLength(255)]
    public string Description {get;set;} = string.Empty;

    /// <summary>
    /// Уникальный код транзакции для начала загрузки.
    /// </summary>
    [Required]
    public long StartPosition {get; set;}

    /// <summary>
    /// Размер паки
    /// </summary>
    [Required]
    public long BatchSize {get;set;} = 1000;
}
