using Microsoft.EntityFrameworkCore;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;
using PersonalAccount.Data.Extensions;

namespace PersonalAccount.Data.Extensions;

/// <summary>
/// Реализация сервиса выделения новых сущностей.
/// </summary>
public class EntityExtractor : IEntityExtractor
{
    private readonly PersonalAccountContext _context;

    public EntityExtractor(PersonalAccountContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmploeeModel>> ExtractNewEmployeesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var inCodes = rows
            .Where(r => r.EmploeeCode.HasValue)
            .Select(r => r.EmploeeCode!.Value)
            .Distinct()
            .ToList();

        if (!inCodes.Any()) return Enumerable.Empty<EmploeeModel>();

        var existCodes = await _context.Emploees
            .Where(e => inCodes.Contains(e.Code))
            .Select(e => e.Code)
            .ToListAsync(token);

        var newCodes = inCodes.Except(existCodes).ToList();

        return newCodes.Select(code => {
            var row = rows.First(r => r.EmploeeCode == code);
            return new EmploeeModel
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = row.EmploeeName ?? $"Сотрудник {code}",
                Phone = ""
            };
        });
    }

    public async Task<IEnumerable<CategoryModel>> ExtractNewCategoriesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var inCodes = rows
            .Where(r => r.CategoryCode.HasValue)
            .Select(r => r.CategoryCode!.Value)
            .Distinct()
            .ToList();

        if (!inCodes.Any()) return Enumerable.Empty<CategoryModel>();

        var existCodes = await _context.Categories
            .Where(c => inCodes.Contains(c.Code))
            .Select(c => c.Code)
            .ToListAsync(token);

        var newCodes = inCodes.Except(existCodes).ToList();

        return newCodes.Select(code => {
            var row = rows.First(r => r.CategoryCode == code);
            return new CategoryModel
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = row.CategoryName ?? $"Категория #{code}"
            };
        });
    }

    public async Task<IEnumerable<NomenclatureModel>> ExtractNewNomenclatureAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var inCodes = rows
        .Where(r => r.ProductCode.HasValue)
        .Select(r => r.ProductCode!.Value)
        .Distinct()
        .ToList();

        if (!inCodes.Any()) return Enumerable.Empty<NomenclatureModel>();

        var existCodes = await _context.Nomenclatures
        .Where(e => inCodes.Contains(e.Code))
        .Select(e => e.Code)
        .ToListAsync(token);

        var newCodes = inCodes.Except(existCodes).ToList();

        return newCodes.Select(code => {
            var row = rows.First(r => r.ProductCode == code);
            return new NomenclatureModel
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = row.CategoryName ?? $"Товар #{code}"
            };
        });
    }
}