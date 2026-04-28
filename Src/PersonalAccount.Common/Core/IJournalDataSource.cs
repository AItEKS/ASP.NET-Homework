using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Интерфейс источника данных журнала. 
/// </summary>
public interface IJournalDataSource
{
    /// <summary>
    /// Извлекает набор новых (необработанных) строк журнала на основе настроек загрузки.
    /// </summary>
    /// <param name="branch">Модель филиала, для которого запрашиваются данные.</param>
    /// <param name="batchSize">Количество записей, извлекаемых за один вызов.</param>
    Task<IEnumerable<JournalRowDto>> GetUnprocessedRowsAsync(BranchModel branch, int batchSize, CancellationToken token);
}