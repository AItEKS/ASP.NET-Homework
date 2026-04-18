using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Company
{
    public Guid Id { get; set; }

    public string? Inn { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Emploee> Emploees { get; set; } = new List<Emploee>();

    public virtual ICollection<LinksUserCompany> LinksUserCompanies { get; set; } = new List<LinksUserCompany>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
}
