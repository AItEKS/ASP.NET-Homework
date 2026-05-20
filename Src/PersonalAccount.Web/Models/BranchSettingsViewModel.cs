using System;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Models;

/// <summary>
/// Модель данных для формы настроек.
/// </summary>
public class BranchSettingsViewModel
{
    /// <summary>
    /// Список филиалов.
    /// </summary>
    public List<BranchModel> Branches { get; set; } = null!;

    #region  Данные формы

    /// <summary>
    /// Уникальный код филиал
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Уникальный код транзакции для начала загрузки.
    /// </summary>
    public long StartPosition {get; set;}

    /// <summary>
    /// Размер паки
    /// </summary>
    public long BatchSize {get;set;} = 1000;

    #endregion
}
