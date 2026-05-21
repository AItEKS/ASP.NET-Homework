using System;

namespace PersonalAccount.Web.Models;

/// <summary>
/// Базовые параметры формирования отчета.
/// </summary>
public interface IReportSettings
{
    /// <summary>
    /// Уникальный код филиала.
    /// </summary>
    Guid BranchId { get; set; }

    /// <summary>
    /// Начало периода.
    /// </summary>
    DateTime Start { get; set; }

    /// <summary>
    /// Окончание периода.
    /// </summary>
    DateTime Stop { get; set; }
}
