using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? ImportSettings { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<ImportSetting> ImportSettingsNavigation { get; set; } = new List<ImportSetting>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
