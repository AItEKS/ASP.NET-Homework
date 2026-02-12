using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Nomenclature : IId
{
    /// <summary>
    /// ID номенклатуры
    /// </summary>
    public Guid Id { get; set; }
    
    public Guid CategoryId { get; set; }

    public string Name { get; set; }
}