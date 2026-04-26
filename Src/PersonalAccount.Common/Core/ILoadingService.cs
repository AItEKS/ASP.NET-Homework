using System;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

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
        BranchModel branch,
        IEnumerable<JournalRowDto> transactions,
        CancellationToken token);


    /// <summary>
    /// Записать данные
    /// </summary>
    /// <param name="branch"></param>
    /// <param name="transactions"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<bool> PushAsync( 
        BranchModel branch,
        IEnumerable<JournalRowDto> transactions,
        CancellationToken token);
    
    Task<LoadingSettingsModel> GetSettingsAsync(BranchModel branch, CancellationToken token);
}
