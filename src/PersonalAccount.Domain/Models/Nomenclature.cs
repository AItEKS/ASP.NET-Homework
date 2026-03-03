using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Nomenclature : IId
{
    /// <summary>
    /// ID номенклатуры
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Категория товара
    /// </summary>
    public required Category Category { get; set; }

    /// <summary>
    /// Название товара
    /// </summary>
    [ProductName]
    public required string Name { get; set; }

    /// <summary>
    /// Цена товара
    /// </summary>
    [ProductPrice]
    public required decimal Price { get; set; }

    /// <summary>
    /// Единица измерения
    /// </summary>
    [UnitOfMeasure]
    public required string UnitOfMeasure { get; set; }
}