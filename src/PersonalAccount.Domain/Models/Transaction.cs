using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;

namespace PersonalAccount.Domain.Models;

public class Transaction : IId
{
    /// <summary>
    /// ID транзакции
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата и время начала транзакции
    /// </summary>
    public DateTimeOffset StartDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Дата и время завершения транзакции
    /// </summary>
    public DateTimeOffset? EndDate { get; set; }

    /// <summary>
    /// Тип транзакции (покупка, продажа)
    /// </summary>
    public required TransactionType Type { get; set; }

    /// <summary>
    /// Статус транзакции
    /// </summary>
    public required TransactionStatus Status { get; set; }
}

/// <summary>
/// Тип транзакции
/// </summary>
public enum TransactionType
{
    /// <summary>Продажа</summary>
    Sell = 0,

    /// <summary>Покупка</summary>
    Buy = 1,
}


/// <summary>
/// Статус транзакции
/// </summary>
public enum TransactionStatus
{
    /// <summary>В ожидании</summary>
    Pending = 0,

    /// <summary>В обработке</summary>
    Processing = 1,

    /// <summary>Завершена успешно</summary>
    Completed = 2,

    /// <summary>Отменена</summary>
    Cancelled = 3,

    /// <summary>Ошибка</summary>
    Failed = 4
}