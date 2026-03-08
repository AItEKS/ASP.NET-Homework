using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Nomenclature
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string UnitOfMeasure { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
