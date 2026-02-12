using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Organization : IId
{
    /// <summary>
    /// ID организации
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Название организации 
    /// </summary>
    [OrgName]
    public string Name { get; set; } = null!;

    /// <summary>
    /// ИНН организации 
    /// </summary>
    [Inn]
    public string Inn { get; set; } = null!;

    /// <summary>
    /// Адрес организации 
    /// </summary>
    [OrgAddress]
    public required string Address { get; set; }
}