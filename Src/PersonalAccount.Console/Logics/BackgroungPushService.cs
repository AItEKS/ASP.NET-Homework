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
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;
using Serilog;

namespace PersonalAccount.Console.Logics;

public class BackgroungPushService : BackgroundService
{
    private readonly PersonalAccount.Common.Models.ConsoleOptions _options;
    private readonly IClientRepository<JournalRowDto> _repo;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ApiClient _apiClient;

    public BackgroungPushService(
            IOptions<PersonalAccount.Common.Models.ConsoleOptions> options,
            IClientRepository<JournalRowDto> repo,
            IHttpClientFactory httpClientFactory,
            ApiClient apiClient)
    {
        _options = options.Value;
        _repo = repo;
        _httpClientFactory = httpClientFactory;
        _apiClient = apiClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information($"[ФОН] Служба запущена для филиала: {_options.BranchId}");

        long currentPos = 0; 

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_options.ServerHost);
                
                var url = $"console/{_options.BranchId}";
                var response = await client.GetAsync(url, stoppingToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var settings = await response.Content.ReadFromJsonAsync<LoadingSettingsModel>(stoppingToken);
                    
                    if (settings != null)
                    {
                        using var connect = new SqlConnection(_options.ConnectionString);
                        await connect.OpenAsync(stoppingToken);

                        var transactions = (await _repo.GetRows(connect, settings)).ToList();

                        if (transactions.Any())
                        {
                            Log.Information($"[ФОН] Найдено {transactions.Count} записей. Отправка...");
                            
                            bool success = await _apiClient.SendTransactionsAsync(_options.BranchId, transactions);

                            if (success)
                            {
                                Log.Information($"[ФОН] Успешно отправлено");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[ФОН ОШИБКА] {ex.Message}");
            }
        }
    }
}