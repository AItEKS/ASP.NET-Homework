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
    public List<RevenueReportDto> GetRevenueReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        var finOps = transactions.Where(t => t.OperationType != OperationType.StartWork && t.OperationType != OperationType.EndWork);

        var grouped = finOps.GroupBy(t => t.OperationDate.Date);

        var result = new List<RevenueReportDto>();

        foreach (var dayGroup in grouped)
        {
            var date = dayGroup.Key;

            var dto = new RevenueReportDto
            {
                Period = date,
                OrganizationId = organizationId,
                
                CashAmount = dayGroup.Where(t => t.OperationType == OperationType.Cash).Sum(t => t.Amount),
                NonCashAmount = dayGroup.Where(t => t.OperationType == OperationType.Visa).Sum(t => t.Amount),
                
                OtherAmount = dayGroup.Where(t => 
                    t.OperationType != OperationType.Cash && 
                    t.OperationType != OperationType.Visa).Sum(t => t.Amount),

                DiscountAmount = 0,
                
                IsHoliday = date.DayOfWeek == DayOfWeek.Saturday || 
                            date.DayOfWeek == DayOfWeek.Sunday || 
                            IsHoliday(date)
            };
            result.Add(dto);
        }

        return result.OrderBy(x => x.Period).ToList();
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