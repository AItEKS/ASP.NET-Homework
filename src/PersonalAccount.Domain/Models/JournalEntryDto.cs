using System.ComponentModel.DataAnnotations;
using PersonalAccount.Domain.Validation;

namespace PersonalAccount.Domain.Dto;

/// <summary>
/// DTO для передачи записи журнала (чека) от клиента на сервер
/// </summary>
public class JournalEntryDto
{
    /// <summary>
    /// ID записи
    /// </summary>
    public required long Id { get; set; }

    /// <summary>
    /// ID номера чека
    /// </summary>
    [CheckNumber]
    public required string CheckNumber { get; set; }

    /// <summary>
    /// ID сотрудника
    /// </summary>
    public long? EmployeeCode { get; set; }

    /// <summary>
    /// ID номенклатуры
    /// </summary>
    public long? NomenclatureCode { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [Description]
    public string? Description { get; set; }

    /// <summary>
    /// ID категории
    /// </summary>
    public long? CategoryCode { get; set; }

    /// <summary>
    /// ID операции
    /// </summary>
    public required long OperationCode { get; set; }

    /// <summary>
    /// Дата транзакции с временной зоной
    /// </summary>
    public required DateTimeOffset TransactionDate { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    [NotZero]
    public required decimal Quantity { get; set; }

    /// <summary>
    /// Сумма
    /// </summary>
    [PositiveMoney]
    public required decimal Amount { get; set; }

    /// <summary>
    /// Сумма скидки
    /// </summary>
    public decimal Discount { get; set; }
}