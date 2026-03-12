using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid NomenclatureId { get; set; }

    public Guid EmployeeId { get; set; }

    public DateTime OperationDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public int OperationType { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Nomenclature Nomenclature { get; set; } = null!;
}
