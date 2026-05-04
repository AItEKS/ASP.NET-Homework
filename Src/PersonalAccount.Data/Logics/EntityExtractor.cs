using Microsoft.EntityFrameworkCore;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Data.Logics;

public class EntityExtractor : IEntityExtractor
{
    private readonly PersonalAccountContext _context;
    public EntityExtractor(PersonalAccountContext context) => _context = context;

    public async Task<IEnumerable<EmploeeModel>> ExtractNewEmployeesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var incomingCodes = rows.Where(r => r.EmploeeCode.HasValue).Select(r => r.EmploeeCode!.Value).Distinct().ToList();
        var existingCodes = await _context.Emploees.Where(e => incomingCodes.Contains(e.Code)).Select(e => e.Code).ToListAsync(token);
        var newCodes = incomingCodes.Except(existingCodes);

        return newCodes.Select(code => {
            var row = rows.First(r => r.EmploeeCode == code);
            return new EmploeeModel { Id = Guid.NewGuid(), Code = code, Name = row.EmploeeName ?? $"Emp {code}" };
        });
    }

    public async Task<IEnumerable<CategoryModel>> ExtractNewCategoriesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var incomingCodes = rows.Where(r => r.CategoryCode.HasValue).Select(r => r.CategoryCode!.Value).Distinct().ToList();
        var existingCodes = await _context.Categories.Where(c => incomingCodes.Contains(c.Code)).Select(c => c.Code).ToListAsync(token);
        var newCodes = incomingCodes.Except(existingCodes);

        return newCodes.Select(code => {
            var row = rows.First(r => r.CategoryCode == code);
            return new CategoryModel { Id = Guid.NewGuid(), Code = code, Name = row.CategoryName ?? $"Cat {code}" };
        });
    }

    public async Task<IEnumerable<NomenclatureModel>> ExtractNewNomenclatureAsync(IEnumerable<JournalRowDto> rows, CancellationToken token)
    {
        var incomingCodes = rows.Where(r => r.ProductCode.HasValue).Select(r => r.ProductCode!.Value).Distinct().ToList();
        var existingCodes = await _context.Nomenclatures.Where(n => incomingCodes.Contains(n.Code)).Select(n => n.Code).ToListAsync(token);
        var newCodes = incomingCodes.Except(existingCodes);

        return newCodes.Select(code => {
            var row = rows.First(r => r.ProductCode == code);
            return new NomenclatureModel { Id = Guid.NewGuid(), Code = code, Name = row.ProductName ?? $"Prod {code}" };
        });
    }
}