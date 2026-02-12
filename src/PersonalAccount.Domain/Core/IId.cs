using System;

namespace PersonalAccount.Domain.Core;

/// <summary>
/// Общий интерфейс для работы с моделями
/// </summary>
public interface IId
{
    /// <summary>
    /// Универсальный код
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Наименование ктегории
    /// </summary>
    public string Name { get; set; }
}