using System;
using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IWorkScheduleReportService"/>
/// </summary>
public class WorkScheduleReportService : IWorkScheduleReportService
{
    // Структура для ключа
    private record WorkScheduleKey (DateTime Period, Guid EmploeeId);

    /// <summary>
    /// Сформировать отчет
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <returns></returns>
    public IEnumerable<WorkScheduleDto> Create(IEnumerable<TransactionModel> transactions)
    {
        if(!transactions.Any()) return  Enumerable.Empty<WorkScheduleDto>() ;
        var companyId = transactions.FirstOrDefault()?.Owner.Id ?? throw new InvalidOperationException("Невозможно получить код корганизации!");

        // Получаем все старты в разрезе каждого дня
        var starting =  Task.Run( () => 
                         transactions
                        .Where(x => x.Type == Domain.Core.TransactionType.StartShift)
                        .GroupBy(
                            x => new WorkScheduleKey(x.Period.Date, x.Emploee.Id),
                            x => x,
                            (key, group) => new
                            {
                                Key = key,
                                Transactions = group.ToList()
                            })
                            .ToDictionary(x => x.Key, x => x.Transactions));

        // Получаем все окончания 
        var stopping = Task.Run( () => 
                         transactions
                        .Where(x => x.Type == Domain.Core.TransactionType.StopShift)
                        .GroupBy(
                            x => new WorkScheduleKey(x.Period.Date, x.Emploee.Id),
                            x => x,
                            (key, group) => new
                            {
                                Key = key,
                                Transactions = group.ToList()
                            })
                            .ToDictionary(x => x.Key, x => x.Transactions));


        // Все сотрудники
        var emploees = Task.Run( () =>
                      transactions
                     .Where(x => x.Type == Domain.Core.TransactionType.StartShift || 
                                x.Type == Domain.Core.TransactionType.StopShift)
                     .Select(x => x.Emploee)
                     .GroupBy(x => new { x.Id })
                     .Select(g => g.First())
                     .ToList());

        // Все периоды
        var periods = Task.Run( () =>             
                     transactions
                    .Where(x => x.Type == Domain.Core.TransactionType.StartShift || 
                               x.Type == Domain.Core.TransactionType.StopShift)
                    .Select(x => x.Period)
                    .Distinct());    

        // Ожидаем расчета
        Task.WaitAll( starting, stopping, emploees, periods);       

        // Объединяем записи
        var items = emploees.Result.SelectMany(employee =>
                        periods.Result.Select(period => new { Employee = employee, Period = period }));

        // Формируем результат
        var result = items.Select( x => new WorkScheduleDto()
        {
            EmploeeId = x.Employee.Id,
            EmploeeName = x.Employee.Name,
            Start = starting.Result.TryGetValue(new WorkScheduleKey(x.Period.Date, x.Employee.Id), out var startingKey)
                   // Берем минимальное значение даты
                   ? startingKey.Min(t => t.Period).Date
                   // Или начало дня
                   : x.Period,

            Stop = stopping.Result.TryGetValue( new WorkScheduleKey(x.Period.Date, x.Employee.Id), out var stoppingKey)
                    // Берем максимальное значение даты
                    ? stoppingKey.Max(t => t.Period).Date
                    // Или окончание дня
                    : x.Period.AddDays(1).AddSeconds(-1),

            CompanyId = companyId
        });
        
        return result
                .GroupBy(x => new { x.CompanyId, x.EmploeeName })
                .Select(g => g.First());
    }


    /// <summary>
    /// Реализация ассинхронного варианта
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<WorkScheduleDto>> CreateAsync(IEnumerable<TransactionModel> transactions, CancellationToken token)
         => await Task.Run( () => Create( transactions), token);
}
