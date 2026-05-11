using System;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Models;

/// <summary>
/// Модель данных для формы настроек.
/// </summary>
public class BranchSettingsModel
{
    /// <summary>
    /// Список филиалов.
    /// </summary>
    public List<BranchModel> Branches { get; set; } = new();

    /// <summary>
    /// Выбранный филиал.
    /// </summary>
    public BranchModel Branch { get; set; } = null!;

    /// <summary>
    /// Текст ошибки валидации.
    /// </summary>
    public string? ErrorText { get; set; }

    /// <summary>
    /// Сообщение об успешном сохранении.
    /// </summary>
    public string? SuccessText { get; set; }
}
