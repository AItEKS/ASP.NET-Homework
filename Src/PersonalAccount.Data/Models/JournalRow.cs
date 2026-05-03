using System;
using System.Collections.Generic;

namespace PersonalAccount.Data.Models;

public partial class Journal
{
    public long? Transnumber { get; set; }

    public long? Transtype { get; set; }

    public long? Receiptn { get; set; }

    public long? Productid { get; set; }

    public string? ProductName { get; set; }

    public long? Categoryid { get; set; }

    public string? CategoryName { get; set; }

    public long? Emploeeid { get; set; }

    public string? EmploeeName { get; set; }

    public DateTime? Dater { get; set; }

    public double? Quantity { get; set; }

    public double? Price { get; set; }

    public double? Discountamount { get; set; }

    public Guid CompanyId { get; set; }

    public Guid BranchId { get; set; }
}
