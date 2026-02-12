using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Category : IId
{
    /// <summary>
    /// ID категории
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID родителя
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// Название ктегории
    /// </summary>
    [CategoryName]
    public required string Name { get; set; }
}