using NUnit.Framework;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Services;
using PersonalAccount.Domain.Dto.Reports;

namespace PersonalAccount.UnitTests;

[TestFixture]
public class ReportRepositoryTests
{
    private Organization _testOrg;
    private Employee _testEmployee;
    private Nomenclature _testProduct;
    private ReportRepo _repository;

    [SetUp]
    public void Setup()
    {
        _testOrg = new Organization 
        { 
            Id = Guid.NewGuid(), 
            Name = "ООО ЯНДЕКС", 
            Inn = "7736207543", 
            Address = "г. Москва, ул. Льва Толстого, д. 16" 
        };

        _testEmployee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Name = "Иванов Иван Иванович", 
            Organization = new List<Organization> { _testOrg }, 
            Phone = "+79001234567" 
        };

        var category = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Продукты питания", 
            Parent = null 
        };

        _testProduct = new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Name = "Молоко Домик в Деревне 3.2%", 
            Price = 100, 
            UnitOfMeasure = "шт", 
            Category = category 
        };

        _repository = new ReportRepo();
    }

    [Test]
    public void GetRevenueReport_ShouldCalculateTotalsCorrectly()
    {
        // Подготовка
        var date = new DateTimeOffset(new DateTime(2023, 10, 01, 12, 00, 00), TimeSpan.Zero);
        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.Cash, 100, 1, date),
            CreateTransaction(OperationType.Cash, 50, 1, date),
            CreateTransaction(OperationType.Visa, 200, 1, date),
            CreateTransaction(OperationType.StartWork, 1, 1, date)
        };

        // Действие
        var report = _repository.GetRevenueReport(transactions, _testOrg.Id);

        // Проверка
        Assert.That(report, Is.Not.Empty);
        var row = report.First();
        Assert.That(row.CashAmount, Is.EqualTo(150));
        Assert.That(row.NonCashAmount, Is.EqualTo(200));
        Assert.That(row.OrganizationId, Is.EqualTo(_testOrg.Id));
    }

    [Test]
    public void GetSalesReport_ShouldGroupByNomenclature()
    {
        // Подготовка
        var date = DateTimeOffset.UtcNow;
        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.PluSales, 200, 2, date),
            CreateTransaction(OperationType.PluSales, 100, 1, date),
            CreateTransaction(OperationType.Cash, 300, 1, date) 
        };

        // Действие
        var report = _repository.GetSalesReport(transactions, _testOrg.Id);

        // Проверка
        Assert.That(report, Is.Not.Empty);
        Assert.That(report.Count, Is.EqualTo(1));
        
        var row = report.First();
        Assert.That(row.NomenclatureName, Is.EqualTo("Молоко Домик в Деревне 3.2%"));
        Assert.That(row.Quantity, Is.EqualTo(3));
        Assert.That(row.Amount, Is.EqualTo(300));
    }

    [Test]
    public void GetWorkScheduleReport_ShouldPairStartAndEndWork()
    {
        // Подготовка
        var day = new DateTime(2023, 10, 01);
        var startTs = new DateTimeOffset(day.AddHours(9), TimeSpan.Zero);
        var endTs = new DateTimeOffset(day.AddHours(18), TimeSpan.Zero);

        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.StartWork, 1, 1, startTs),
            CreateTransaction(OperationType.PluSales, 100, 1, startTs.AddHours(1)),
            CreateTransaction(OperationType.EndWork, 1, 1, endTs)
        };

        // Действие
        var report = _repository.GetWorkScheduleReport(transactions, _testOrg.Id);

        // Проверка
        Assert.That(report, Is.Not.Empty);
        var row = report.First();
        Assert.That(row.EmployeeCode, Is.EqualTo(_testEmployee.Id));
        Assert.That(row.StartWork, Is.EqualTo(startTs));
        Assert.That(row.EndWork, Is.EqualTo(endTs));
    }

    private Transaction CreateTransaction(OperationType type, decimal amount, decimal qty, DateTimeOffset date)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Employee = _testEmployee,
            Nomenclature = _testProduct,
            OperationType = type,
            Amount = amount,
            Quantity = qty,
            OperationDate = date,
            CreatedAt = DateTime.UtcNow
        };
    }
}