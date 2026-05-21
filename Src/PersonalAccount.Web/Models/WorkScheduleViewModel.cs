using System;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Web.Models;

/// <summary>
/// Модель данных страницы отчета "График работы".
/// </summary>
public class WorkScheduleViewModel : IReportSettings
{
    /// <summary>
    /// Список филиалов.
    /// </summary>
    public List<BranchModel> Branches { get; set; } = new();

    /// <inheritdoc/>
    public Guid BranchId { get; set; }

    /// <inheritdoc/>
    public DateTime Start { get; set; } = DateTime.UtcNow.Date.AddDays(-7);

    /// <inheritdoc/>
    public DateTime Stop { get; set; } = DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

    /// <summary>
    /// Сформированный отчет.
    /// </summary>
    public List<WorkScheduleDto> Rows { get; set; } = new();
}
