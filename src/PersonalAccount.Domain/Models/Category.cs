using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Category : IId
{
    /// <summary>
    /// ID категории
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Родительская категория (может быть null, если это корневая категория)
    /// </summary>
    public Category? Parent { get; set; }

    /// <summary>
    /// Название категории
    /// </summary>
    [CategoryName]
    public required string Name { get; set; }
}