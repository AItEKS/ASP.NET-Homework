using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Employee : IId
{
    /// <summary>
    /// ID сотрудника 
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Имя сотрудника 
    /// </summary>
    [EmployeeName]
    public required string Name { get; set; }
    
    /// <summary>
    /// Телефон сотрудника 
    /// </summary>
    [EmployeePhone]
    public string? Phone { get; set; }

    /// <summary>
    /// Организация
    /// </summary>
    public required Organization Organization { get; set; }
}