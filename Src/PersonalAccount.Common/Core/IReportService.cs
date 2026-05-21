using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Сервис-фабрика построения отчетов.
/// На входе - тип отчета, на выходе - DTO с нужным отчетом.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Получить набор транзакций по филиалу за период.
    /// </summary>
    /// <param name="branchId"> Уникальный код филиала. </param>
    /// <param name="start"> Начало периода. </param>
    /// <param name="end"> Окончание периода. </param>
    IEnumerable<TransactionModel> Get(Guid branchId, DateTime start, DateTime end);

    /// <summary>
    /// Асинхронный вариант <see cref="Get"/>.
    /// </summary>
    Task<IEnumerable<TransactionModel>> GetAsync(Guid branchId, DateTime start, DateTime end, CancellationToken token);

    /// <summary>
    /// Сформировать отчет указанного типа на основании набора транзакций.
    /// </summary>
    /// <typeparam name="T"> Тип DTO отчета. </typeparam>
    /// <param name="transactions"> Транзакции. </param>
    /// <param name="reportType"> Тип отчета. </param>
    IEnumerable<T> Create<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType) where T : IDto;

    /// <summary>
    /// Асинхронный вариант <see cref="Create{T}"/>.
    /// </summary>
    Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType, CancellationToken token) where T : IDto;
}
