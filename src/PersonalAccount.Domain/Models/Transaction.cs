using System;
using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Validation;

namespace PersonalAccount.Domain.Models;

/// <summary>
/// Тип операции с товаром/деньгами
/// </summary>
public enum OperationType
{
    Sale = 1,       // Продажа
    Return = 2,     // Возврат
    WriteOff = 3    // Списание
}

/// <summary>
/// Тип источника для импорта
/// </summary>
public enum ImportSourceType
{
    Excel = 1,
    Csv = 2,
    Xml = 3,
    OneC = 4 // 1C
}

public class Transaction
{
    /// <summary>
    /// ID транзакции
    /// </summary>
    public long Id { get; set; }

    /// <summary> 
    /// Номенклатура 
    /// </summary>
    public required Nomenclature Nomenclature { get; set; }

    /// <summary>
    /// Ответственный сотрудник
    /// </summary>
    public required Employee Employee { get; set; }

    /// <summary>
    /// Дата совершения операции (на чеке)
    /// </summary>
    [PastOrPresent] 
    public required DateTimeOffset OperationDate { get; set; }

    /// <summary>
    /// Дата регистрации в системе
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Количество товара
    /// </summary>
    [NotZero]
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Сумма операции
    /// </summary>
    [PositiveMoney]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Тип операции (Продажа, Возврат, Списание)
    /// </summary>
    public required OperationType OperationType { get; set; }
}