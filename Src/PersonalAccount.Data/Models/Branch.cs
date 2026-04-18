using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Branch
{
    public Guid Id { get; set; }

    public Guid CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? LoadOptions { get; set; }

    public virtual Company Company { get; set; } = null!;
}