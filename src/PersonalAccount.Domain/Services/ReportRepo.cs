using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Dto.Reports;

namespace PersonalAccount.Domain.Services;

public class ReportRepo : IReportRepository
{
    private readonly HashSet<(int Month, int Day)> _holidays = new()
    {
        (1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6), (1, 7), (1, 8),
        (2, 23), (3, 8), (5, 1), (5, 9), (6, 12), (11, 4)
    };

    /// <summary>
    /// Функция для определения рабочего дня
    /// </summary>
    /// <param name="dateToCheck"></param>
    /// <returns></returns>
    private bool IsHoliday(DateTimeOffset dateToCheck)
    {
        return _holidays.Contains((dateToCheck.Month, dateToCheck.Day));
    }

    /// <summary>
    /// Отчет о выручке
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public IEnumerable<RevenueReportDto> GetRevenueReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        // Все скидки
        var calcDiscountTask =  Task.Run( () =>
                                transactions
                                .GroupBy(x => x.OperationDate.Date)
                                .Select(x => new {
                                    Key  = x.Key,
                                    Value = x.Sum(t => t.Discount)
                                })
                                .ToDictionary(x => x.Key, x => x.Value));

        // Рассчитать все банковские оплаты
        var calcBankTask = Task.Run( () =>
        {
            var allDiscounts = transactions
                            .Where(x => x.OperationType == OperationType.Visa)
                            .GroupBy(x => x.OperationDate.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Discount)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            var allPayments = transactions
                            .Where(x => x.OperationType == OperationType.Visa)
                            .GroupBy(x => x.OperationDate.Date) 
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Amount * t.Quantity)
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
                            .Where(x => x.OperationType == OperationType.Cash)
                            .GroupBy(x => x.OperationDate.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Discount)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);

            var allPayments = transactions
                            .Where(x => x.OperationType == OperationType.Cash)
                            .GroupBy(x => x.OperationDate.Date) 
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Amount * t.Quantity)
                            })
                            .ToDictionary(x => x.Key, x => x.Value);


            var allRefunds = transactions
                            .Where(x => x.OperationType == OperationType.Refund)
                            .GroupBy(x => x.OperationDate.Date)
                            .Select(x => new {
                                Key  = x.Key,
                                Value = x.Sum(t => t.Amount * t.Quantity)
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
        var result = periods.Select( x => new RevenueReportDto()
        {
            Period = x,
            NonCashAmount = calcBankTask.Result.ContainsKey( x ) ? calcBankTask.Result[ x ] : 0,
            CashAmount = calcCashTask.Result.ContainsKey( x ) ? calcCashTask.Result[ x ] : 0,
            DiscountAmount = calcDiscountTask.Result.ContainsKey( x ) ? calcDiscountTask.Result[ x ] : 0,
            OrganizationId = transactions.FirstOrDefault()?.OrganizationId ?? Guid.Empty
        });

        return result ;  
    }

    /// <summary>
    /// Отчет о продажах
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public List<SalesReportDto> GetSalesReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        var sales = transactions.Where(t => t.OperationType == OperationType.PluSales);

        return sales
            .GroupBy(t => new { 
                NomId = t.Nomenclature.Id, 
                NomName = t.Nomenclature.Name, 
                CatId = t.Nomenclature.Category.Id, 
                CatName = t.Nomenclature.Category.Name 
            })
            .Select(g => new SalesReportDto
            {
                OrganizationId = organizationId,
                NomenclatureCode = g.Key.NomId,
                NomenclatureName = g.Key.NomName,
                GroupCode = g.Key.CatId,
                GroupName = g.Key.CatName,
                
                Quantity = g.Sum(t => t.Quantity),
                Amount = g.Sum(t => t.Amount),
                DiscountAmount = 0
            })
            .OrderBy(x => x.GroupName)
            .ThenBy(x => x.NomenclatureName)
            .ToList();
    }

    /// <summary>
    /// Отчет о графике работы
    /// </summary>
    /// <param name="transactions"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public List<WorkScheduleReportDto> GetWorkScheduleReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        var result = new List<WorkScheduleReportDto>();

        var workOps = transactions
            .Where(t => t.OperationType == OperationType.StartWork || t.OperationType == OperationType.EndWork)
            .OrderBy(t => t.OperationDate)
            .ToList();

        var empSched = new Dictionary<Guid, WorkScheduleReportDto>();
        
        foreach (var workOp in workOps)
        {
            if (workOp.OperationType == OperationType.StartWork)
            {
                if (empSched.ContainsKey(workOp.Employee.Id)) 
                {
                    empSched.Remove(workOp.Employee.Id); 
                }
    
                var dto = new WorkScheduleReportDto
                {
                    OrganizationId = organizationId,
                    EmployeeCode = workOp.Employee.Id,
                    Name = workOp.Employee.Name,
                    StartWork = workOp.OperationDate
                };

                empSched.Add(workOp.Employee.Id, dto);
            } 
            else if (workOp.OperationType == OperationType.EndWork)
            {
                if (empSched.TryGetValue(workOp.Employee.Id, out var dto))
                {
                    empSched.Remove(workOp.Employee.Id);
                    dto.EndWork = workOp.OperationDate;
                    result.Add(dto);
                }
            }
        }

        return result;
    }
}