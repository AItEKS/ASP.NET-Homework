using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalAccount.Domain.Models;

/// <summary>
/// Модель номенклатуры.
/// </summary>
public class NomenclatureModel : DomainModel
{
    /// <summary>
    /// Наименование,
    /// </summary>
    [Required]
    public string Name {get;set;} = string.Empty;

    /// <summary>
    /// Категория.
    /// </summary>
    [Required]
    public CategoryModel Category {get;set;} = null!;

    /// <summary>
    /// Внешний код номенклатуры (из клиентской базы MS SQL).
    /// Используется для синхронизации.
    /// </summary>
    [Required]
    public long Code { get; set; }
}
