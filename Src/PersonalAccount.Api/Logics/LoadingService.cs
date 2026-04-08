using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Logics;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

public class LoadingService : ILoadingService
{
    private readonly ICompanySettingsRepository _settingReposity;
    
    private readonly IJournalRowRepository _journalRowRepository; 
  
    public LoadingService(
        ICompanySettingsRepository settingsRepository, 
        IJournalRowRepository journalRowRepository)
    {
        _settingReposity = settingsRepository;
        _journalRowRepository = journalRowRepository;
    }

    public bool Push(CompanyModel company, IEnumerable<JournalRowDto> transactions, CancellationToken token)
    {
        var settings = _settingReposity.LoadAsync(company, token).Result
                        ?? new LoadingSettingsModel()
                        {
                            Owner = company, StartPosition = 1, BatchSize = 1000
                        };

        settings.Owner = company;

        var firstTransaction = transactions.FirstOrDefault();
        if(firstTransaction is null) return false;
        
        var innerTransactions = transactions.Where(x => x.Code >= settings.StartPosition).ToList();

        if (!innerTransactions.Any()) 
            return true;

        _journalRowRepository.BulkInsertAsync(innerTransactions, token).Wait();
        
        var lastCode = innerTransactions.OrderByDescending(x => x.Code).First().Code;
        
        settings.StartPosition = lastCode + 1; 
        
        var task = Task.Run(() => _settingReposity.SaveAsync(settings, token), token);
        Task.WaitAll(task);
    
        return true;
    }

    public async Task<bool> PushAsync(CompanyModel company, IEnumerable<JournalRowDto> transactions, CancellationToken token)
        => await Task.Run(() => Push(company, transactions, token), token);
}