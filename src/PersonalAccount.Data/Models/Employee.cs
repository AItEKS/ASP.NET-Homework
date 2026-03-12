using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Employee
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
