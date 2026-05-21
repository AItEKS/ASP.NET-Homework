using Microsoft.EntityFrameworkCore;
using PersonalAccount.Common.Core;
using PersonalAccount.Data;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Web.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IReportService"/>.
/// Фабрика отчетов: на входе - тип отчета, на выходе - DTO с нужным отчетом.
/// </summary>
public class ReportService(
    IRevenueReportService revenueReport,
    ISalesReportService sallingReport,
    IWorkScheduleReportService workScheduleReport,
    PersonalAccountContext context) : IReportService
{
    private readonly IRevenueReportService _revenueReport = revenueReport;
    private readonly ISalesReportService _sallingReport = sallingReport;
    private readonly IWorkScheduleReportService _workScheduleReport = workScheduleReport;
    private readonly PersonalAccountContext _context = context;

    /// <inheritdoc/>
    public IEnumerable<T> Create<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType) where T : IDto
    {
        var result = reportType switch
        {
            ReportTypeEnum.Revenue => _revenueReport.Create(transactions) as IEnumerable<T>,
            ReportTypeEnum.Salling => _sallingReport.Create(transactions) as IEnumerable<T>,
            ReportTypeEnum.WorkSchedule => _workScheduleReport.Create(transactions) as IEnumerable<T>,
            _ => throw new NotImplementedException($"Тип отчета {reportType} не поддерживается!")
        };
        return result!;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> CreateAsync<T>(IEnumerable<TransactionModel> transactions, ReportTypeEnum reportType, CancellationToken token) where T : IDto
        => await Task.Run(() => Create<T>(transactions, reportType), token);

    /// <inheritdoc/>
    public IEnumerable<TransactionModel> Get(Guid branchId, DateTime start, DateTime end)
    {
        var startUtc = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        var endUtc = DateTime.SpecifyKind(end, DateTimeKind.Utc);

        var branch = _context.Branches
            .Include(b => b.Company)
            .FirstOrDefault(b => b.Id == branchId)
            ?? throw new InvalidOperationException($"Невозможно найти филиал по коду {branchId}!");

        var rows = _context.Transactions
            .Include(t => t.Emloee)
            .Include(t => t.Nomenclature)
                .ThenInclude(n => n!.Category)
            .Where(t => t.BranchId == branchId
                        && t.ChangePeriod >= startUtc
                        && t.ChangePeriod <= endUtc)
            .ToList();

        var company = new CompanyModel
        {
            Id = branch.Company.Id,
            Name = branch.Company.Name ?? string.Empty,
            INN = branch.Company.Inn ?? string.Empty,
            Address = branch.Company.Address ?? string.Empty
        };

        var branchModel = new BranchModel
        {
            Id = branch.Id,
            Name = branch.Name ?? string.Empty,
            Owner = company
        };

        return rows.Select(t => new TransactionModel
        {
            Id = t.Id,
            Type = (TransactionType)t.TransactionType,
            Period = new DateTimeOffset(DateTime.SpecifyKind(t.ChangePeriod, DateTimeKind.Utc)),
            Price = (double)(t.Price ?? 0m),
            Quantuty = (double)(t.Quantity ?? 0m),
            Discount = (double)(t.Discount ?? 0m),
            Owner = company,
            Branch = branchModel,
            Emploee = t.Emloee is null
                ? new EmploeeModel { Id = Guid.Empty, Name = string.Empty, Owner = company }
                : new EmploeeModel
                {
                    Id = t.Emloee.Id,
                    Name = t.Emloee.Name ?? string.Empty,
                    ExternalCode = t.Emloee.ExternalCode,
                    Owner = company
                },
            Nomenclature = t.Nomenclature is null
                ? new NomenclatureModel
                {
                    Id = Guid.Empty,
                    Name = string.Empty,
                    Category = new CategoryModel { Id = Guid.Empty, Name = string.Empty, Owner = company }
                }
                : new NomenclatureModel
                {
                    Id = t.Nomenclature.Id,
                    Name = t.Nomenclature.Name ?? string.Empty,
                    ExternalCode = t.Nomenclature.ExternalCode,
                    Category = t.Nomenclature.Category is null
                        ? new CategoryModel { Id = Guid.Empty, Name = string.Empty, Owner = company }
                        : new CategoryModel
                        {
                            Id = t.Nomenclature.Category.Id,
                            Name = t.Nomenclature.Category.Name ?? string.Empty,
                            ExternalCode = t.Nomenclature.Category.ExternalCode,
                            Owner = company
                        }
                }
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TransactionModel>> GetAsync(Guid branchId, DateTime start, DateTime end, CancellationToken token)
        => await Task.Run(() => Get(branchId, start, end), token);
}
