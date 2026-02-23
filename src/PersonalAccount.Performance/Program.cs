using PersonalAccount.Console.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

/// <summary>
/// Метод для замера времени выполнения
/// </summary>
static void Measure(string operationName, Action action)
{
    GC.Collect();
    GC.WaitForPendingFinalizers();

    Console.WriteLine($"Начало: {operationName}");
    
    var stopwatch = Stopwatch.StartNew();
    
    try
    {
        action();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка: {ex.Message}");
    }
    
    stopwatch.Stop();
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Время выполнения: {stopwatch.Elapsed}");
    Console.ResetColor();
    Console.WriteLine(new string('-', 30));
}

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

var configuration = builder.Build();
var options = configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException("Не удалось прочитать appsettings.json");

var provider = new JournalProvider(options.ConnectionString);

DateTime baseDate = new DateTime(2020, 11, 10); 
DateTime startDay = baseDate.Date;
DateTime endDay = baseDate.Date.AddDays(1).AddSeconds(-1);
DateTime startMonth = new DateTime(baseDate.Year, baseDate.Month, 1);
DateTime endMonth = startMonth.AddMonths(1).AddSeconds(-1);
int quarterNumber = (baseDate.Month - 1) / 3 + 1;
DateTime startQuarter = new DateTime(baseDate.Year, (quarterNumber - 1) * 3 + 1, 1);
DateTime endQuarter = startQuarter.AddMonths(3).AddSeconds(-1);

try {
    provider.GetTransactions(DateTime.Now, DateTime.Now); 
} catch {}

Measure("Загрузка за ДЕНЬ", () => 
{
    var data = provider.GetTransactions(startDay, endDay);
    Console.WriteLine($"Кол-во записей: {data.Count}");
});

Measure("Загрузка за МЕСЯЦ", () => 
{
    var data = provider.GetTransactions(startMonth, endMonth);
    Console.WriteLine($"Кол-во записей: {data.Count}");
});

Measure("Загрузка за КВАРТАЛ", () => 
{
    var data = provider.GetTransactions(startQuarter, endQuarter);
    Console.WriteLine($"Кол-во записей: {data.Count}");
});
