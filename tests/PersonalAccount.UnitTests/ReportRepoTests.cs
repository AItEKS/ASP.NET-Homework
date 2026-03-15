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
    public void GetRevenueReport_ShouldCalculateTotalsCorrectly_IncludingRefundsAndDiscounts()
    {
        // Подготовка
        var date = new DateTimeOffset(new DateTime(2023, 10, 01, 12, 00, 00), TimeSpan.Zero);
        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.Cash, 100, 1, date, 10),
            CreateTransaction(OperationType.Cash, 50, 2, date, 15),
            CreateTransaction(OperationType.Refund, 20, 1, date, 5),
            CreateTransaction(OperationType.Visa, 200, 1, date, 20),
            CreateTransaction(OperationType.StartWork, 1, 1, date, 10)
        };

        // Действие
        var report = _repository.GetRevenueReport(transactions, _testOrg.Id).ToList();

        // Проверка
        Assert.That(report, Is.Not.Empty);
        var row = report.First();

        Assert.That(row.CashAmount, Is.EqualTo(155));
        Assert.That(row.NonCashAmount, Is.EqualTo(180));
        Assert.That(row.DiscountAmount, Is.EqualTo(60));
        Assert.That(row.OrganizationId, Is.EqualTo(_testOrg.Id));
        Assert.That(row.Period.Date, Is.EqualTo(date.Date));
    }

    [Test]
    public void GetSalesReport_ShouldGroupByNomenclatureAndIncludeCategories()
    {
        // Подготовка
        var date = DateTimeOffset.UtcNow;
        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.PluSales, 200, 2, date, 0),
            CreateTransaction(OperationType.PluSales, 100, 1, date, 0),
            CreateTransaction(OperationType.Cash, 300, 1, date, 0)
        };

        // Действие
        var report = _repository.GetSalesReport(transactions, _testOrg.Id);

        // Проверка
        Assert.That(report, Is.Not.Empty);
        Assert.That(report.Count, Is.EqualTo(1));
        
        var row = report.First();
        Assert.That(row.NomenclatureName, Is.EqualTo("Молоко Домик в Деревне 3.2%"));
        Assert.That(row.NomenclatureCode, Is.EqualTo(_testProduct.Id));
        Assert.That(row.GroupName, Is.EqualTo("Продукты питания"));
        Assert.That(row.GroupCode, Is.EqualTo(_testProduct.Category.Id));
        Assert.That(row.Quantity, Is.EqualTo(3));
        Assert.That(row.Amount, Is.EqualTo(300));
        Assert.That(row.DiscountAmount, Is.EqualTo(0));
    }

    [Test]
    public void GetWorkScheduleReport_ShouldPairStartAndEndWorkWithEmployeeDetails()
    {
        // Подготовка
        var day = new DateTime(2023, 10, 01);
        var startTs = new DateTimeOffset(day.AddHours(9), TimeSpan.Zero);
        var endTs = new DateTimeOffset(day.AddHours(18), TimeSpan.Zero);

        var transactions = new List<Transaction>
        {
            CreateTransaction(OperationType.StartWork, 1, 1, startTs, 0),
            CreateTransaction(OperationType.PluSales, 100, 1, startTs.AddHours(1), 0),
            CreateTransaction(OperationType.EndWork, 1, 1, endTs, 0)
        };

        // Действие
        var report = _repository.GetWorkScheduleReport(transactions, _testOrg.Id);

        // Проверка
        Assert.That(report, Is.Not.Empty);
        var row = report.First();
        
        Assert.That(row.EmployeeCode, Is.EqualTo(_testEmployee.Id));
        Assert.That(row.Name, Is.EqualTo(_testEmployee.Name));
        Assert.That(row.StartWork, Is.EqualTo(startTs));
        Assert.That(row.EndWork, Is.EqualTo(endTs));
        Assert.That(row.OrganizationId, Is.EqualTo(_testOrg.Id));
    }

    private Transaction CreateTransaction(OperationType type, decimal amount, decimal qty, DateTimeOffset date, decimal discount)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            OrganizationId = _testOrg.Id,
            Employee = _testEmployee,
            Nomenclature = _testProduct,
            OperationType = type,
            Amount = amount,
            Quantity = qty,
            Discount = discount,
            OperationDate = date,
            CreatedAt = DateTime.UtcNow
        };
    }
}