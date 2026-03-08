namespace PersonalAccount.Domain.Dto.Reports;

public class SalesReportDto
{
    public Guid GroupCode { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public Guid NomenclatureCode { get; set; }
    public string NomenclatureName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid OrganizationId { get; set; }
}