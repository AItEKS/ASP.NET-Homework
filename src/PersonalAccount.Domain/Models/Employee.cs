using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Employee : IId
{
    /// <summary>
    /// ID сотрудника 
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Наименование сотрудника 
    /// </summary>
    [Length(10, 255)]
    public required string Name { get; set; }
    
    /// <summary>
    /// Телефон сотрудника 
    /// </summary>
    [PhoneTemplate("^\\+?[1-9][0-9]{7,14}$")]
    public string? Phone { get; set; }
}