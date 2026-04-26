using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

public interface IJournalDataSource
{
    // Получает набор новых строк
    Task<IEnumerable<JournalRowDto>> GetUnprocessedRowsAsync(int batchSize, CancellationToken token);
}