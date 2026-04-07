using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Common.Core;

public interface IJournalRowRepository
{
    /// <summary>
    /// Скоростная массовая вставка сырых строк журнала в базу данных.
    /// </summary>
    Task BulkInsertAsync(IEnumerable<JournalRowDto> rows, CancellationToken token);
}