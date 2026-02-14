using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Category : IId
{
    /// <summary>
    /// ID категории
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// ID родителя
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// Название ктегории
    /// </summary>
    [CategoryName]
    public required string Name { get; set; }
}