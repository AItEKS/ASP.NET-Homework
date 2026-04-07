using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalAccount.Data.Models;

[Table("journal_rows")]
public class JournalRow
{
    [Key]
    [Column("code")]
    public long Code { get; set; }

    [Column("type_code")]
    public long TypeCode { get; set; }

    [Column("receipt_number")]
    public long ReceiptNumber { get; set; }[Column("product_code")]
    public long? ProductCode { get; set; }[Column("category_code")]
    public long? CategoryCode { get; set; }[Column("emploee_code")]
    public long? EmploeeCode { get; set; }[Column("emploee_name")]
    public string? EmploeeName { get; set; }[Column("category_name")]
    public string? CategoryName { get; set; }[Column("nomenclature_name")]
    public string? NomenclatureName { get; set; }

    [Column("period")]
    public DateTime Period { get; set; }

    [Column("quantity")]
    public double Quantity { get; set; }

    [Column("price")]
    public double Price { get; set; }[Column("discount")]
    public double Discount { get; set; }

    [Column("uploaded_at")]
    public DateTime UploadedAt { get; set; }
}