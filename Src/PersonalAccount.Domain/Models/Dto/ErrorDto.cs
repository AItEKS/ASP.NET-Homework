using System;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models.Dto;

/// <summary>
/// Описание ошибки
/// </summary>
public class ErrorDto : IErrorText
{
    // Флаг
    public bool IsError => 
        !string.IsNullOrWhiteSpace(ErrorText);

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string ErrorText { get; set;} = null!;

    /// <summary>
    /// Стек вызова.
    /// </summary>
    public string StackTrace { get; set;} = null!;
}
