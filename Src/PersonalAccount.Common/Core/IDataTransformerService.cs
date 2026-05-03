using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Сервис преобразования сырых строк журнала в доменные модели транзакций.
/// </summary>
public interface IDataTransformerService
{
    /// <summary>
    /// Преобразует DTO в доменные модели, связывая их с существующими объектами справочников.
    /// </summary>
    IEnumerable<TransactionModel> MapToDomain(
        BranchModel branch,
        IEnumerable<JournalRowDto> rows,
        IEnumerable<EmploeeModel> employees,
        IEnumerable<NomenclatureModel> nomenclature);
}