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
    private readonly IHttpClientFactory _httpClientFactory;

    public BackgroungPushService(
            IOptions<ConsoleOptions> options,
            IClientRepository<JournalRowDto> repo,
            ApiClient apiClient,
            IHttpClientFactory httpClientFactory)
    {
        _options = options.Value;
        _repo = repo;
        _apiClient = apiClient;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        System.Console.WriteLine($"[ФОН] Служба запущена. Филиал: {_options.BranchId}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(_options.ApiUrl);
                
                var settingsUrl = $"api/journal/settings/{_options.BranchId}";
                var remoteSettings = await client.GetFromJsonAsync<LoadingSettingsModel>(settingsUrl, stoppingToken);

                if (remoteSettings == null) 
                {
                    System.Console.WriteLine("! Не удалось получить настройки от сервера.");
                    continue;
                }

                System.Console.WriteLine($"[ФОН] Сервер ждет данные с позиции: {remoteSettings.StartPosition}");

                using var connect = new SqlConnection(_options.MsSqlConnectionString);
                await connect.OpenAsync(stoppingToken);

                var transactions = (await _repo.GetRows(connect, remoteSettings)).ToList();

                if (transactions.Any())
                {
                    System.Console.WriteLine($"[ФОН] Найдено {transactions.Count} новых чеков. Отправка...");

                    bool success = await _apiClient.SendTransactionsAsync(_options.BranchId, transactions);
                    
                    if (success)
                        System.Console.WriteLine($"[ФОН] Пакет успешно доставлен.");
                }
                else
                {
                    System.Console.WriteLine("[ФОН] Новых данных на кассе нет.");
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[ФОН ОШИБКА] {ex.Message}");
            }
        }
    }
}