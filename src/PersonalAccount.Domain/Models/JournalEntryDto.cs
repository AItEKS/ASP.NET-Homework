using PersonalAccount.Domain.Validation;

namespace PersonalAccount.Domain.Dto;

/// <summary>
/// DTO для передачи записи журнала
/// </summary>
public class JournalEntryDto
{
    /// <summary>
    /// ID записи
    /// </summary>
    [DbColumn("transnumber", typeof(int))]
    public int Id { get; set; }

    /// <summary>
    /// ID номера чека
    /// </summary>
    [DbColumn("receiptn", typeof(int))]
    public int CheckNumber { get; set; }

    /// <summary>
    /// ID сотрудника
    /// </summary>
    [DbColumn("CalcEmployee", typeof(int))]
    public int? EmployeeCode { get; set; }

    /// <summary>
    /// ID номенклатуры
    /// </summary>
    [DbColumn("CalcNomenclature", typeof(string))]
    public string? NomenclatureCode { get; set; }

    /// <summary>
    /// Описание
    /// </summary>
    [DbColumn("description", typeof(string))]
    public string? Description { get; set; }

    /// <summary>
    /// ID категории
    /// </summary>
    public long? CategoryCode { get; set; }

    /// <summary>
    /// ID операции
    /// </summary>
    [DbColumn("transtype", typeof(int))]
    public int OperationCode { get; set; }

    /// <summary>
    /// Дата транзакции с временной зоной
    /// </summary>
    [DbColumn("dater", typeof(DateTime))]
    public DateTimeOffset TransactionDate { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    [NotZero]
    [DbColumn("quantity", typeof(decimal))]
    public decimal Quantity { get; set; }

    /// <summary>
    /// Сумма
    /// </summary>
    [PositiveMoney]
    [DbColumn("amount", typeof(decimal))]
    public decimal Amount { get; set; }

    /// <summary>
    /// Сумма скидки
    /// </summary>
    [DbColumn("discountamount", typeof(decimal))]
    public decimal Discount { get; set; }
}