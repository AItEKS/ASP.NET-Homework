using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.Domain.Models;

/// <summary>
/// Модель категории номераклатуры.
/// </summary>
public class CategoryModel : DomainModel
{
    /// <summary>
    /// Наименование категории.
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Организация владелец категории.
    /// </summary>
    [Required]
    public CompanyModel Owner {get;set;} = null!;

    /// <summary>
    /// Внешний код категории (из клиентской базы MS SQL).
    /// Используется для синхронизации.
    /// </summary>
    [Required]
    public long Code { get; set; }
}
