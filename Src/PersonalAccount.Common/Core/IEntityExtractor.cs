using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

public interface IEntityExtractor
{
    // Находит категории, которых нет в справочнике
    Task<IEnumerable<CategoryModel>> ExtractNewCategoriesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);
    
    // Находит товары, которых нет в справочнике
    Task<IEnumerable<NomenclatureModel>> ExtractNewNomenclatureAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);
    
    // Находит сотрудников, которых нет в справочнике
    Task<IEnumerable<EmploeeModel>> ExtractNewEmployeesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);
}