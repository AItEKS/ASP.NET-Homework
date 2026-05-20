using PersonalAccount.Common.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Api.Logics;

/// <summary>
/// Реализация интерфейса <see cref="IRevenueReportService"/>
/// </summary>
public class RevenueReportService : IRevenueReportService
{
    /// <summary>
    /// Сформировать отчет
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <returns></returns>
    public IEnumerable<RevenueDto> Create(IEnumerable<TransactionModel> transactions)
    {
        if(!transactions.Any()) return  Enumerable.Empty<RevenueDto>() ;

        // Все скидки
        var calcDiscountTask =  Task.Run( () =>
                                transactions
                                .GroupBy(x => x.Period.Date)
                                .Select(x => new {
                                    Key  = x.Key,
                                    Value = x.Sum(t => t.Discount)
                                })
                                .ToDictionary(x => x.Key, x => x.Value));
     
        // Рассчитать все банковские оплаты
        var calcBankTask = Task.Run( () =>
        {
            var allDiscounts = transactions
                            .Where(x => x.Type == Domain.Core.TransactionType.BankPayment)
                            .GroupBy(x => x.Period.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Discount)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            var allPayments = transactions
                            .Where(x => x.Type == Domain.Core.TransactionType.BankPayment)
                            .GroupBy(x => x.Period.Date) 
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Price * t.Quantuty)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            return allPayments.ToDictionary(
                pair => pair.Key,
                pair => pair.Value
                        - (allDiscounts.ContainsKey(pair.Key) ?  allDiscounts[ pair.Key ] : 0)
            );
        });

        // Рассчитать все оплаты наличными
        var calcCashTask = Task.Run( () =>
        {
            var allDiscounts = transactions
                            .Where(x => x.Type == Domain.Core.TransactionType.CashPayment)
                            .GroupBy(x => x.Period.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Discount)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            var allPayments = transactions
                            .Where(x => x.Type == Domain.Core.TransactionType.CashPayment)
                            .GroupBy(x => x.Period.Date) 
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Price * t.Quantuty)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);


            var allRefunds = transactions
                            .Where(x => x.Type == Domain.Core.TransactionType.RefundPayment)
                            .GroupBy(x => x.Period.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Price * t.Quantuty)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            return allPayments.ToDictionary(
                pair => pair.Key,
                pair => pair.Value 
                        - (allDiscounts.ContainsKey(pair.Key) ?  allDiscounts[pair.Key]  : 0)
                        - (allRefunds.ContainsKey(pair.Key) ? allRefunds[pair.Key] : 0)
            );                
        });

        // Ожидаем расчета
        Task.WaitAll( calcBankTask, calcCashTask, calcDiscountTask);

        // Получим список всех дат
        var periods = calcBankTask.Result.Keys
                    .Union( calcCashTask.Result.Keys )
                    .Union( calcDiscountTask.Result.Keys )
                    .Distinct()
                    .ToList();

        // Формируем результат
        var result = periods.Select( x => new RevenueDto()
        {
            Period = x,
            BankAmount = calcBankTask.Result.ContainsKey( x ) ? calcBankTask.Result[ x ] : 0,
            CashAmount = calcCashTask.Result.ContainsKey( x ) ? calcCashTask.Result[ x ] : 0,
            DiscountAmount = calcDiscountTask.Result.ContainsKey( x ) ? calcDiscountTask.Result[ x ] : 0,
            Owner = transactions.FirstOrDefault()?.Owner.Id ?? Guid.Empty
        }).OrderBy(x => x.Period);

        return result ;            
    }

    /// <summary>
    /// Реализация ассинхронного варианта
    /// </summary>
    /// <param name="transactions"> Набор транзакций. </param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<IEnumerable<RevenueDto>> CreateAsync(IEnumerable<TransactionModel> transactions, CancellationToken token)
        => await Task.Run( () => Create( transactions), token);
}
