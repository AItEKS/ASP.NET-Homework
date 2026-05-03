using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Nomenclature
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public long Code { get; set; }
}
