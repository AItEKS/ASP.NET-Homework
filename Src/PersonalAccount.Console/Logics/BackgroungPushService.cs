using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PersonalAccount.Common.Core;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Console.Logics;

public class BackgroungPushService : BackgroundService
{
    private readonly ConsoleOptions _options;
    private readonly IClientRepository<JournalRowDto> _repo;
    private readonly ApiClient _apiClient;
    
    private long _currentStartPosition = 0;

    public BackgroungPushService(
            IOptions<ConsoleOptions> options,
            IClientRepository<JournalRowDto> repo,
            ApiClient apiClient)
    {
        _options = options.Value;
        _repo = repo;
        _apiClient = apiClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        System.Console.WriteLine($"[ФОН] Запущен для филиала: {_options.BranchId}");

        long currentPos = 0; 

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var connect = new SqlConnection(_options.MsSqlConnectionString);
                await connect.OpenAsync(stoppingToken);

                var settings = new LoadingSettingsModel { 
                    BatchSize = _options.BatchSize, 
                    StartPosition = currentPos 
                };

                var transactions = (await _repo.GetRows(connect, settings)).ToList();

                if (transactions.Any())
                {
                    System.Console.WriteLine($"[ФОН] Найдено {transactions.Count} чеков. Отправка...");

                    bool success = await _apiClient.SendTransactionsAsync(_options.BranchId, transactions);

                    if (success)
                    {
                        currentPos = transactions.Max(x => x.Code) + 1;
                        System.Console.WriteLine($"[ФОН] Успешно. Следующий ID: {currentPos}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ФОН ОШИБКА] {ex.Message}");
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}