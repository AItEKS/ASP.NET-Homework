using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Dto.Reports;

namespace PersonalAccount.Domain;

public class ReportRepo : IReportRepository
{
    public List<RevenueReportDto> GetRevenueReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        var finOps = transactions.Where(t => t.OperationType != OperationType.StartWork && t.OperationType != OperationType.EndWork);

        var grouped = finOps.GroupBy(t => t.OperationDate);

        var result = new List<RevenueReportDto>();

        foreach (var dayGroup in grouped)
        {
            var dto = new RevenueReportDto
            {
                Period = dayGroup.Key,
                OrganizationId = organizationId,
                
                CashAmount = dayGroup.Where(t => t.OperationType == OperationType.Cash).Sum(t => t.Amount),
                NonCashAmount = dayGroup.Where(t => t.OperationType == OperationType.Visa).Sum(t => t.Amount),
                
                OtherAmount = dayGroup.Where(t => 
                    t.OperationType != OperationType.Cash && 
                    t.OperationType != OperationType.Visa).Sum(t => t.Amount),

                DiscountAmount = 0,
                
                IsHoliday = dayGroup.Key.DayOfWeek == DayOfWeek.Saturday || dayGroup.Key.DayOfWeek == DayOfWeek.Sunday
            };
            result.Add(dto);
        }

        return result.OrderBy(x => x.Period).ToList();
    }

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

    public List<WorkScheduleReportDto> GetWorkScheduleReport(IEnumerable<Models.Transaction> transactions, Guid organizationId)
    {
        var result = new List<WorkScheduleReportDto>();

        var workOps = transactions
            .Where(t => t.OperationType == OperationType.StartWork || t.OperationType == OperationType.EndWork)
            .OrderBy(t => t.OperationDate)
            .ToList();

        var employeeGroups = workOps.GroupBy(t => t.Employee.Id);

        foreach (var empGroup in employeeGroups)
        {
            var empName = empGroup.First().Employee.Name;
            var empOps = empGroup.ToList();

            for (int i = 0; i < empOps.Count; i++)
            {
                var currentOp = empOps[i];

                if (currentOp.OperationType == OperationType.StartWork)
                {
                    var dto = new WorkScheduleReportDto
                    {
                        OrganizationId = organizationId,
                        EmployeeCode = empGroup.Key,
                        Name = empName,
                        StartWork = currentOp.OperationDate
                    };

                    if (i + 1 < empOps.Count && empOps[i + 1].OperationType == OperationType.EndWork)
                    {
                        dto.EndWork = empOps[i + 1].OperationDate;
                        i++;
                    }

                    result.Add(dto);
                }
            }
        }

        return result;
    }
}