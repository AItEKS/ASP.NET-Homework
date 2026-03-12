using System;

namespace PersonalAccount.Domain.Dto.Reports;

public class RevenueReportDto
{
    public DateTimeOffset Period { get; set; }

    public decimal CashAmount { get; set; }

    public decimal NonCashAmount { get; set; }

    public decimal OtherAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public bool IsHoliday { get; set; }

    public Guid OrganizationId { get; set; }
}