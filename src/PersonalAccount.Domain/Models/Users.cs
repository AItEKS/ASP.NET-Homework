namespace PersonalAccount.Domain.Models;

public class Users
{
    /// <summary>
    /// ID пользователя 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Логин 
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Пароль
    /// </summary>
    public required string PasswordHash { get; set; }

    /// <summary>
    /// Роль
    /// </summary>
    public required string Role { get; set; }
}