using System.Diagnostics;
using PersonalAccount.Performance.Services;

namespace PersonalAccount.Domain.Services;

public class ReportsPerformanceProfiler
{
    private readonly ReportRepo _reportRepo;

    public ReportsPerformanceProfiler(ReportRepo reportRepo)
    {
        _reportRepo = reportRepo ?? throw new ArgumentNullException(nameof(reportRepo));
    }

    public void RunBenchmarks(DateTime baseDate)
    {
        System.Console.WriteLine($"Сравнение генерации отчетов для даты: {baseDate:dd.MM.yyyy}");
        var orgId = Guid.NewGuid();
        int[] datasetSizes = { 100, 1_000, 100_000 };

        var warmupData = FakeDataGenerator.GenerateTransactions(10, baseDate, orgId);
        _reportRepo.GetRevenueReportSync(warmupData, orgId).ToList();
        _reportRepo.GetRevenueReport(warmupData, orgId).ToList();
        
        foreach (var size in datasetSizes)
        {
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            System.Console.ResetColor();
            
            var fakeData = FakeDataGenerator.GenerateTransactions(size, baseDate, orgId);

            Measure($"GetRevenueReportSync ({size:N0} чеков)", () =>
            {
                var report = _reportRepo.GetRevenueReportSync(fakeData, orgId).ToList();
            });

            Measure($"GetRevenueReport ({size:N0} чеков)", () =>
            {
                var report = _reportRepo.GetRevenueReport(fakeData, orgId).ToList();
            });
        }
    }

    private void Measure(string operationName, Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        System.Console.WriteLine($"Начало: {operationName}");
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            action();
        }
        catch (Exception ex)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine($"Время выполнения: {stopwatch.Elapsed.TotalMilliseconds:F4} мс");
            System.Console.ResetColor();
            System.Console.WriteLine(new string('-', 50));
        }
    }
}