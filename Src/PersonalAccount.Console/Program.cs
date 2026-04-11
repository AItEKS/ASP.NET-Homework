using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
                     ?? throw new InvalidOperationException("Настройки ConsoleOptions не найдены!");

services.AddTransient<IClientRepository<JournalRowDto>, JournalRepository>();

services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(consoleOptions.ApiUrl);
    client.Timeout = TimeSpan.FromMinutes(2);
});

var serviceProvider = services.BuildServiceProvider();

CurrentApplication.ShowLogo();
Console.WriteLine("Запуск консольного клиента выгрузки...");
Console.WriteLine($"API: {consoleOptions.ApiUrl}");

var journalRepo = serviceProvider.GetRequiredService<IClientRepository<JournalRowDto>>();
var apiClient = serviceProvider.GetRequiredService<ApiClient>();

var syncSettings = new LoadingSettingsModel 
{ 
    BatchSize = consoleOptions.BatchSize, 
    StartPosition = 0 
};

using var sqlConnection = new SqlConnection(consoleOptions.MsSqlConnectionString);
await sqlConnection.OpenAsync();

var totalStopwatch = Stopwatch.StartNew();
var totalRecordsSent = 0;
var cycleNumber = 0;

while (true)
{
    cycleNumber++;
    
    try
    {
        Console.WriteLine($"\n[Цикл #{cycleNumber}] Чтение данных из кассы (начиная с ID {syncSettings.StartPosition})...");
        
        var rows = await journalRepo.GetRows(sqlConnection, syncSettings);

        if (rows != null && rows.Any())
        {
            var count = rows.Count();
            Console.WriteLine($"Найдено {count} новых записей. Отправка на сервер...");
            
            bool isSuccess = await apiClient.SendTransactionsAsync(consoleOptions.CompanyId, rows);

            if (isSuccess)
            {
                var maxId = rows.Max(r => r.Code);
                syncSettings.StartPosition = maxId + 1;
                totalRecordsSent += count;
                
                Console.WriteLine($"✓ Успешно отправлено {count} записей");
                Console.WriteLine($"  Прогресс: {totalRecordsSent} записей за {totalStopwatch.Elapsed:hh\\:mm\\:ss}");
            }
            else
            {
                Console.WriteLine($"✗ Ошибка отправки данных");
            }
        }
        else
        {
            totalStopwatch.Stop();
            
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("ПЕРЕНОС ДАННЫХ ЗАВЕРШЕН");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Всего перенесено записей: {totalRecordsSent}");
            Console.WriteLine($"Общее время переноса: {totalStopwatch.Elapsed:hh\\:mm\\:ss\\.fff}");
            Console.WriteLine($"Количество циклов: {cycleNumber}");
            Console.WriteLine($"Средняя скорость: {(totalRecordsSent / totalStopwatch.Elapsed.TotalSeconds):F2} записей/сек");
            Console.WriteLine(new string('=', 60));
            
            break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Ошибка в цикле синхронизации: {ex.Message}");
    }
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();