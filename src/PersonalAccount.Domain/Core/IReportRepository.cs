using System.Collections.Generic;
using PersonalAccount.Domain.Dto.Reports;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Domain.Core;

public interface IReportRepository
{
    /// <summary>
    /// Формирует отчет по выручке
    /// </summary>
    IEnumerable<RevenueReportDto> GetRevenueReport(IEnumerable<Transaction> transactions, Guid organizationId);

    /// <summary>
    /// Формирует отчет по продажам товаров
    /// </summary>
    List<SalesReportDto> GetSalesReport(IEnumerable<Transaction> transactions, Guid organizationId);

    /// <summary>
    /// Формирует график работы (смены)
    /// </summary>
    List<WorkScheduleReportDto> GetWorkScheduleReport(IEnumerable<Transaction> transactions, Guid organizationId);
}