using System;
using PersonalAccount.Domain.Dto;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Common.Core;

/// <summary>
/// Сервис загрузки данных из клиента.
/// </summary>
public interface ILoadingService
{
    /// <summary>
    /// Записать данные
    /// </summary>
    /// <param name="company"></param>
    /// <param name="transactions"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public bool Push( 
        Organization company,
        IEnumerable<JournalEntryDto> transactions,
        string connectionString,
        CancellationToken token);


    /// <summary>
    /// Записать данные
    /// </summary>
    /// <param name="company"></param>
    /// <param name="transactions"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<bool> PushAsync( 
        Organization company,
        IEnumerable<JournalEntryDto> transactions,
        string connectionString,
        CancellationToken token);    
}
