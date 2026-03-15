using System.Diagnostics;

namespace PersonalAccount.Performance;

public class TransactionPerformanceProfiler
{
    private readonly JournalProvider _provider;

    public TransactionPerformanceProfiler(JournalProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public void RunBenchmarks(DateTime baseDate)
    {
        System.Console.WriteLine($"Запуск бенчмарка для даты: {baseDate:dd.MM.yyyy}");

        WarmUp();

        var (startDay, endDay) = GetDayRange(baseDate);
        var (startMonth, endMonth) = GetMonthRange(baseDate);
        var (startQuarter, endQuarter) = GetQuarterRange(baseDate);

        Measure("Загрузка за ДЕНЬ", () =>
        {
            var data = _provider.GetTransactions(startDay, endDay);
            System.Console.WriteLine($"Кол-во записей: {data.Count}");
        });

        Measure("Загрузка за МЕСЯЦ", () =>
        {
            var data = _provider.GetTransactions(startMonth, endMonth);
            System.Console.WriteLine($"Кол-во записей: {data.Count}");
        });

        Measure("Загрузка за КВАРТАЛ", () =>
        {
            var data = _provider.GetTransactions(startQuarter, endQuarter);
            System.Console.WriteLine($"Кол-во записей: {data.Count}");
        });
    }

    private void WarmUp()
    {
        try
        {
            _provider.GetTransactions(DateTime.Now, DateTime.Now);
        }
        catch 
        {
        }
    }

    private void Measure(string operationName, Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        System.Console.WriteLine($"\nНачало: {operationName}");
        
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
            System.Console.WriteLine($"Время выполнения: {stopwatch.Elapsed}");
            System.Console.ResetColor();
            System.Console.WriteLine(new string('-', 30));
        }
    }

    private (DateTime Start, DateTime End) GetDayRange(DateTime date)
    {
        DateTime start = date.Date;
        DateTime end = start.AddDays(1).AddSeconds(-1);
        return (start, end);
    }

    private (DateTime Start, DateTime End) GetMonthRange(DateTime date)
    {
        DateTime start = new DateTime(date.Year, date.Month, 1);
        DateTime end = start.AddMonths(1).AddSeconds(-1);
        return (start, end);
    }

    private (DateTime Start, DateTime End) GetQuarterRange(DateTime date)
    {
        int quarterNumber = (date.Month - 1) / 3 + 1;
        DateTime start = new DateTime(date.Year, (quarterNumber - 1) * 3 + 1, 1);
        DateTime end = start.AddMonths(3).AddSeconds(-1);
        return (start, end);
    }
}