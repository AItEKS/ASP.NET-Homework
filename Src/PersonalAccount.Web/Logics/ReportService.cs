using System;
using Microsoft.Identity.Client.Extensibility;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IReportService"/>
/// </summary>
public class ReportService(IRevenueReportService revenueReport, ISallingReportService sallingReport, IWorkScheduleReportService workScheduleReport) : IReportService
{
    // Сервис для построения отчета "Выручка"
    private readonly IRevenueReportService _revenueReport = revenueReport;

    // Сервис для построения отчета "Продажи"
    private readonly ISallingReportService _sallingReport = sallingReport;

    // Сервис для построения отчета "График работы"
    private readonly IWorkScheduleReportService _workScheduleReport = workScheduleReport;

    /// <inheritdoc/>
    public IEnumerable<T> Create<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType) where T : IDto
    {
        var result = reportType switch
        {
            ReportTypeEnum.Revenue => _revenueReport.Create(transactions) as IEnumerable<T>,
            ReportTypeEnum.Salling => _sallingReport.Create(transactions) as IEnumerable<T>,
            ReportTypeEnum.WorkSchedule => _workScheduleReport.Create(transactions) as IEnumerable<T>,
            _ => throw new NotImplementedException()
        };
        return result!;
    }


    /// <inheritdoc/>
    public async Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType, CancellationToken token) where T : IDto
        => await Task.Run( () => Create<T>(transactions, reportType), token);

    public IEnumerable<TransactionModel> Get(Guid branchId, DateTime start, DateTime end)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionModel>> GetAsync(Guid branchId, DateTime start, DateTime end, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
