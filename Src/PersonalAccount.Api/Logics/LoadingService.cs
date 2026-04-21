using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

public class LoadingService : ILoadingService
{
    private readonly IBranchSettingsRepository _settingRepository;
    private readonly IJournalRowRepository _journalRowRepository; 
  
    public LoadingService(
        IBranchSettingsRepository settingsRepository, 
        IJournalRowRepository journalRowRepository)
    {
        _settingRepository = settingsRepository;
        _journalRowRepository = journalRowRepository;
    }

    public bool Push(BranchModel branch, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        var settings = _settingRepository.LoadAsync(branch, token).Result
                        ?? new LoadingSettingsModel()
                        {
                            Owner = branch, StartPosition = 1, BatchSize = 1000
                        };

        settings.Owner = branch;

        var firstTransaction = transactions.FirstOrDefault();
        if(firstTransaction is null) return false;
        
        var innerTransactions = transactions.Where(x => x.Code >= settings.StartPosition).ToList();

        if (!innerTransactions.Any()) 
            return true;

        _journalRowRepository.BulkInsertAsync(innerTransactions, token).Wait();
        
        var lastCode = innerTransactions.OrderByDescending(x => x.Code).First().Code;
        
        settings.StartPosition = lastCode + 1; 
        
        var task = Task.Run(() => _settingRepository.SaveAsync(settings, token), token);
        Task.WaitAll(task);
    
        return true;
    }

    public async Task<bool> PushAsync(BranchModel branch, IEnumerable<JournalRowDto> transactions, CancellationToken token)
        => await Task.Run(() => Push(branch, transactions, token), token);

    public async Task<LoadingSettingsModel> GetSettingsAsync(BranchModel branch, CancellationToken token)
    {
        return await _settingRepository.LoadAsync(branch, token);
    }
}