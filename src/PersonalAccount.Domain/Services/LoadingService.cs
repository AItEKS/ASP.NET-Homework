using System;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Dto;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Api.Logics;

public class LoadingService : ILoadingService
{
    public bool Push(Organization organization, IEnumerable<JournalEntryDto> transactions, string connectionString, CancellationToken token)
    {
        // 1 Поучаем настройки
        var settingsRepo = new LoadingSettingsRepo(connectionString);
        var settings = settingsRepo.Load(organization, token).Result;
        if (!settings) return false;

        var firstTransaction = transactions.FirstOrDefault();
        if(firstTransaction is null) return false;
        
        // Отбрасываем лишние
        var innerTransactions = transactions.Where(x => x.Id >= organization.Settings.StartPosition);

        // Сохраняем 
        
        // Обновляем настройки
        var lastCode = innerTransactions.OrderByDescending(x => x.Id).First().Id;
        organization.Settings.StartPosition = lastCode;
        var task = Task.Run( () =>  settingsRepo.Save(organization, token), token);
        Task.WaitAll( task );
    
        return true;
    }

    public async Task<bool> PushAsync(Organization organization, IEnumerable<JournalEntryDto> transactions, string connectionString, CancellationToken token)
        => await Task.Run( () => Push(organization, transactions, connectionString, token), token);
}