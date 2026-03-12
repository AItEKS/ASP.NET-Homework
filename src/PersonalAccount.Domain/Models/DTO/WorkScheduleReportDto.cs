using System;

namespace PersonalAccount.Domain.Dto.Reports;

public class WorkScheduleReportDto
{
    public Guid EmployeeCode { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset StartWork { get; set; }
    public DateTimeOffset? EndWork { get; set; }
    public Guid OrganizationId { get; set; }
}