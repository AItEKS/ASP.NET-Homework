using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Category
{
    public Guid Id { get; set; }

    public Guid? ParentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual ICollection<Nomenclature> Nomenclatures { get; set; } = new List<Nomenclature>();

    public virtual Category? Parent { get; set; }
}
