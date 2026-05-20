using System;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Web.Models;

/// <summary>
/// Модель представление ошибки
/// </summary>
public class ErrorViewModel : IErrorText
{
    /// <summary>
    /// Заголовок ошибки
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string ErrorText { get; set; } = null!;

    /// <summary>
    /// Стек вызова
    /// </summary>
    public string StackTrace { get; set;} = null!;

    /// <summary>
    /// Флаг наличие ошибки
    /// </summary>
    public bool IsError => !string.IsNullOrWhiteSpace(ErrorText);
}
