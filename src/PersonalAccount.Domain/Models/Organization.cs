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
    public required string Name { get; set; }

    /// <summary>
    /// ИНН организации 
    /// </summary>
    [Inn]
    public required string Inn { get; set; }

    /// <summary>
    /// Адрес организации 
    /// </summary>
    [OrgAddress]
    public required string Address { get; set; }

    /// <summary>
    /// Настройки загрузки 
    /// </summary>
    public ImportSettings? Settings { get; set; }
}