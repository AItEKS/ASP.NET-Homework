using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersonalAccount.Common.Core;
using PersonalAccount.Console.Logics;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

services.Configure<ConsoleOptions>(configuration.GetSection(nameof(ConsoleOptions)));
var consoleOptions = configuration.GetSection(nameof(ConsoleOptions)).Get<ConsoleOptions>()
                     ?? throw new InvalidOperationException("Настройки не найдены!");

services.AddTransient<IClientRepository<JournalRowDto>, JournalRepository>();

services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(consoleOptions.ApiUrl);
    client.Timeout = TimeSpan.FromMinutes(2);
});

var serviceProvider = services.BuildServiceProvider();

CurrentApplication.ShowLogo();
Console.WriteLine("Запуск консольного клиента выгрузки...");
Console.WriteLine($"Подключение к API: {consoleOptions.ApiUrl}");

var journalRepo = serviceProvider.GetRequiredService<IClientRepository<JournalRowDto>>();
var apiClient = serviceProvider.GetRequiredService<ApiClient>();

var syncSettings = new LoadingSettingsModel { BatchSize = 1000, StartPosition = 0 };

using var sqlConnection = new SqlConnection(consoleOptions.MsSqlConnectionString);
await sqlConnection.OpenAsync();

while (true)
{
    try
    {
        Console.WriteLine($"\nЧтение данных из БД (начиная с ID {syncSettings.StartPosition})...");
        
        var rows = await journalRepo.GetRows(sqlConnection, syncSettings);

        if (rows != null && rows.Any())
        {
            Console.WriteLine($"Найдено {rows.Count()} новых записей. Отправка на сервер...");
            
            bool isSuccess = await apiClient.SendTransactionsAsync(consoleOptions.CompanyId, rows);

            if (isSuccess)
            {
                var maxId = rows.Max(r => r.Code);
                syncSettings.StartPosition = maxId + 1;
            }
        }
        else
        {
            Console.WriteLine("Нет новых данных для отправки.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка в цикле: {ex.Message}");
    }
}