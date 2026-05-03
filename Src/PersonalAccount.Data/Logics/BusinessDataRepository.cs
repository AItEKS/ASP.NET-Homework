using EFCore.BulkExtensions;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Models;
using PersonalAccount.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PersonalAccount.Data.Logics;

/// <summary>
/// Реализация репозитория для скоростной записи бизнес-сущностей через Bulk-операции.
/// </summary>
public class BusinessDataRepository : IBusinessDataRepository
{
    private readonly PersonalAccountContext _context;

    public BusinessDataRepository(PersonalAccountContext context)
    {
        _context = context;
    }

    public async Task SaveCategoriesAsync(IEnumerable<CategoryModel> categories, CancellationToken token)
    {
        if (categories == null || !categories.Any()) return;

        var entities = categories.Select(x => new Category
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            CompanyId = x.Owner.Id
        }).ToList();

        await _context.BulkInsertAsync(entities, cancellationToken: token);
    }

    public async Task SaveEmployeesAsync(IEnumerable<EmploeeModel> employees, CancellationToken token)
    {
        if (employees == null || !employees.Any()) return;

        var entities = employees.Select(x => new Emploee
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            CompanyId = x.Owner.Id, 
            Phone = x.Phone ?? string.Empty
        }).ToList();

        await _context.BulkInsertAsync(entities, cancellationToken: token);
    }

    public async Task SaveNomenclatureAsync(IEnumerable<NomenclatureModel> nomenclature, CancellationToken token)
    {
        if (nomenclature == null || !nomenclature.Any()) return;

        var entities = nomenclature.Select(x => new Nomenclature
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            CategoryId = x.Category.Id
        }).ToList();

        await _context.BulkInsertAsync(entities, cancellationToken: token);
    }

    public async Task PrepareTransactionsAsync(IEnumerable<TransactionModel> transactions, CancellationToken token)
    {
        if (!transactions.Any()) return;

        var ids = transactions.Select(x => x.Id).ToList();

        await _context.Transactions
            .Where(t => ids.Contains(t.Id))
            .ExecuteDeleteAsync(token);
    }

    public async Task SaveTransactionsAsync(IEnumerable<TransactionModel> transactions, CancellationToken token)
    {
        if (transactions == null || !transactions.Any()) return;

        var entities = transactions.Select(x => new Transaction
        {
            Id = x.Id,
            TransactionType = (int)x.Type,
            ChangePeriod = x.Period.UtcDateTime,
            NomenclatureId = x.Nomenclature.Id,
            EmloeeId = x.Emploee.Id,
            Price = (decimal)x.Price,
            Quantity = (decimal)x.Quantuty,
            Discount = (decimal)x.Discount,
            BranchId = x.Branch.Id, 
        }).ToList();

        await _context.BulkInsertAsync(entities, cancellationToken: token);
    }
}