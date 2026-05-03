using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Emploee
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public Guid? CompanyId { get; set; }

    public virtual Company? Company { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public long Code { get; set; }
}
