using System;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class BranchModel : IId
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public Guid CompanyId { get; set; }
    public LoadingSettingsModel? Settings { get; set; }
}