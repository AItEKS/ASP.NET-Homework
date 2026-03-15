using Microsoft.Extensions.Configuration;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain.Services;
using PersonalAccount.Performance;

static void GetTransactionsPerformance(ApplicationOptions options)
{
    var provider = new JournalProvider(options.ConnectionString);
    var profiler = new TransactionPerformanceProfiler(provider);

    DateTime baseDate = new DateTime(2020, 11, 10);
    profiler.RunBenchmarks(baseDate);
}

static void GetReportsPerformance(ApplicationOptions options)
{
    var reportRepo = new ReportRepo();
    var reportProfiler = new ReportsPerformanceProfiler(reportRepo);

    DateTime baseDate = new DateTime(2020, 11, 10);
    reportProfiler.RunBenchmarks(baseDate);
}

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var options = configuration.Get<ApplicationOptions>() 
    ?? throw new InvalidOperationException("Не удалось прочитать настройки ApplicationOptions из appsettings.json");

//GetTransactionsPerformance(options);
GetReportsPerformance(options);