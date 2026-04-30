using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Интерфейс сервиса для выделения новых бизнес-сущностей из потока сырых данных.
/// </summary>
public interface IEntityExtractor
{
    /// <summary>
    /// Извлекает из набора транзакций категории номенклатуры, которых еще нет в базе данных.
    /// </summary>
    /// <param name="rows">Набор сырых строк журнала.</param>
    Task<IEnumerable<CategoryModel>> ExtractNewCategoriesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);

    /// <summary>
    /// Извлекает из набора транзакций позиции номенклатуры (товары), которых еще нет в базе данных.
    /// </summary>
    /// <param name="rows">Набор сырых строк журнала.</param>
    Task<IEnumerable<NomenclatureModel>> ExtractNewNomenclatureAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);

    /// <summary>
    /// Извлекает из набора транзакций сотрудников, информация о которых отсутствует в системе.
    /// </summary>
    /// <param name="rows">Набор сырых строк журнала.</param>
    Task<IEnumerable<EmploeeModel>> ExtractNewEmployeesAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);
}