using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NUnit.Framework;
using PersonalAccount.Domain.Dto;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Validation;
using PersonalAccount.UnitTests.Logics; 

namespace PersonalAccount.UnitTests;

[TestFixture]
public class CategoryTests
{
    [Test]
    public void Attributes_Name_Exists()
    {
        // Подготовка
        var type = typeof(Category);

        // Действие
        var attr = type.GetProperty(nameof(Category.Name))?.GetCustomAttribute<CategoryNameAttribute>();

        // Проверка
        Assert.That(attr, Is.Not.Null);
    }

    [Test]
    public void Create_ValidCategory_Success()
    {
        // Подготовка
        var category = new Category { Id = Guid.NewGuid(), Name = "Электроника" };

        // Действие
        var results = ValidationHelper.Validate(category);

        // Проверка
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Create_NameTooShort_Fail()
    {
        // Подготовка
        var category = new Category { Id = Guid.NewGuid(), Name = "A" };

        // Действие
        var results = ValidationHelper.Validate(category);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("от 2 до 100")), Is.True);
    }

    [Test]
    public void Create_NameEmpty_Fail()
    {
        // Подготовка
        var category = new Category { Id = Guid.NewGuid(), Name = "   " };

        // Действие
        var results = ValidationHelper.Validate(category);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("обязательно")), Is.True);
    }
}

[TestFixture]
public class EmployeeTests
{
    private List<Organization> GetValidOrganizations()
    {
        return new List<Organization> 
        {
            new Organization 
            { 
                Id = Guid.NewGuid(), 
                Name = "ООО Рога и Копыта", 
                Inn = "1234567890", 
                Address = "г Москва, ул Пушкина, д 1" 
            }
        };
    }

    [Test]
    public void Attributes_Phone_Exists()
    {
        // Подготовка
        var type = typeof(Employee);

        // Действие
        var attr = type.GetProperty(nameof(Employee.Phone))?.GetCustomAttribute<EmployeePhoneAttribute>();

        // Проверка
        Assert.That(attr, Is.Not.Null);
    }

    [Test]
    public void Create_ValidPhone_Success()
    {
        // Подготовка
        var employee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Organization = GetValidOrganizations(), 
            Name = "Иванов Иван", 
            Phone = "89001234567" 
        };

        // Действие
        var results = ValidationHelper.Validate(employee);

        // Проверка
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Create_NullPhone_Success()
    {
        // Подготовка
        var employee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Organization = GetValidOrganizations(), 
            Name = "Иванов Иван", 
            Phone = null 
        };

        // Действие
        var results = ValidationHelper.Validate(employee);

        // Проверка
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void Create_PhoneLetters_Fail()
    {
        // Подготовка
        var employee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Organization = GetValidOrganizations(), 
            Name = "Иванов", 
            Phone = "8900ABC4567" 
        };

        // Действие
        var results = ValidationHelper.Validate(employee);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("11 цифр")), Is.True);
    }

    [Test]
    public void Create_PhoneBadFormat_Fail()
    {
        // Подготовка
        var employee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Organization = GetValidOrganizations(), 
            Name = "Иванов", 
            Phone = "8_900_123_45_67"
        };

        // Действие
        var results = ValidationHelper.Validate(employee);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Некорректный формат")), Is.True);
    }
}

[TestFixture]
public class OrganizationTests
{
    [Test]
    public void Attributes_All_Exist()
    {
        // Подготовка
        var type = typeof(Organization);

        // Действие
        var innAttr = type.GetProperty(nameof(Organization.Inn))?.GetCustomAttribute<InnAttribute>();
        var addrAttr = type.GetProperty(nameof(Organization.Address))?.GetCustomAttribute<OrgAddressAttribute>();

        // Проверка
        Assert.That(innAttr, Is.Not.Null);
        Assert.That(addrAttr, Is.Not.Null);
    }

    [Test]
    public void Create_InnWithLetters_Fail()
    {
        // Подготовка
        var org = new Organization 
        { 
            Id = Guid.NewGuid(), 
            Name = "ООО Тест", 
            Address = "г Москва, ул Ленина, д 1", 
            Inn = "1234A67890" 
        };

        // Действие
        var results = ValidationHelper.Validate(org);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("только цифры")), Is.True);
    }

    [Test]
    public void Create_InnWrongLength_Fail()
    {
        // Подготовка
        var org = new Organization 
        { 
            Id = Guid.NewGuid(), 
            Name = "ООО Тест", 
            Address = "г Москва, ул Ленина, д 1", 
            Inn = "123" 
        };

        // Действие
        var results = ValidationHelper.Validate(org);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("10 или 12")), Is.True);
    }

    [Test]
    public void Create_AddressLatinCharacters_Fail()
    {
        // Подготовка
        var org = new Organization 
        { 
            Id = Guid.NewGuid(), 
            Name = "ООО Тест", 
            Address = "Some Street 123", 
            Inn = "1234567890" 
        };

        // Действие
        var results = ValidationHelper.Validate(org);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("недопустимые символы") || r.ErrorMessage!.Contains("формат")), Is.True);
    }
}

[TestFixture]
public class NomenclatureTests
{
    private Category GetValidCategory()
    {
        return new Category { Id = Guid.NewGuid(), Name = "Категория 1" };
    }

    [Test]
    public void Attributes_Price_Exists()
    {
        // Подготовка
        var type = typeof(Nomenclature);

        // Действие
        var attr = type.GetProperty(nameof(Nomenclature.Price))?.GetCustomAttribute<ProductPriceAttribute>();

        // Проверка
        Assert.That(attr, Is.Not.Null);
    }

    [Test]
    public void Create_NegativePrice_Fail()
    {
        // Подготовка
        var nom = new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Category = GetValidCategory(),
            Name = "Товар", 
            UnitOfMeasure = "шт", 
            Price = -50m 
        };

        // Действие
        var results = ValidationHelper.Validate(nom);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("не может быть отрицательной")), Is.True);
    }

    [Test]
    public void Create_ZeroPrice_Fail()
    {
        // Подготовка
        var nom = new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Category = GetValidCategory(),
            Name = "Товар", 
            UnitOfMeasure = "шт", 
            Price = 0m 
        };

        // Действие
        var results = ValidationHelper.Validate(nom);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("больше нуля")), Is.True);
    }

    [Test]
    public void Create_InvalidUnit_Fail()
    {
        // Подготовка
        var nom = new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Category = GetValidCategory(),
            Name = "Товар", 
            UnitOfMeasure = "unknown", 
            Price = 10m 
        };

        // Действие
        var results = ValidationHelper.Validate(nom);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Недопустимая единица")), Is.True);
    }
}

[TestFixture]
public class TransactionTests
{
    private Employee GetValidEmployee()
    {
        return new Employee 
        { 
            Id = Guid.NewGuid(), 
            Name = "Иванов", 
            Organization = new List<Organization> 
            { 
                new Organization 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "ООО", 
                    Inn = "1234567890", 
                    Address = "г Москва" 
                } 
            } 
        };
    }

    private Nomenclature GetValidNomenclature()
    {
        return new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Name = "Товар", 
            Price = 100, 
            UnitOfMeasure = "шт",
            Category = new Category { Id = Guid.NewGuid(), Name = "Категория" }
        };
    }

    [Test]
    public void Create_FutureDate_Fail()
    {
        // Подготовка
        var tran = new Transaction 
        { 
            Id = Guid.NewGuid(), 
            Nomenclature = GetValidNomenclature(), 
            Employee = GetValidEmployee(), 
            OperationType = OperationType.Cash, 
            Quantity = 1, 
            Amount = 10,
            Discount = 10,
            OperationDate = DateTimeOffset.Now.AddDays(1) 
        };

        // Действие
        var results = ValidationHelper.Validate(tran);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("в будущем")), Is.True);
    }

    [Test]
    public void Create_ZeroQuantity_Fail()
    {
        // Подготовка
        var tran = new Transaction 
        { 
            Id = Guid.NewGuid(), 
            Nomenclature = GetValidNomenclature(), 
            Employee = GetValidEmployee(), 
            OperationType = OperationType.Cash, 
            Quantity = 0, 
            Amount = 10, 
            Discount = 10,
            OperationDate = DateTimeOffset.Now 
        };

        // Действие
        var results = ValidationHelper.Validate(tran);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("не может быть равно нулю")), Is.True);
    }

    [Test]
    public void Create_NegativeAmount_Fail()
    {
        // Подготовка
        var tran = new Transaction 
        { 
            Id = Guid.NewGuid(), 
            Nomenclature = GetValidNomenclature(), 
            Employee = GetValidEmployee(), 
            OperationType = OperationType.Cash, 
            Quantity = 1, 
            Amount = -100, 
            Discount= 10,
            OperationDate = DateTimeOffset.Now 
        };

        // Действие
        var results = ValidationHelper.Validate(tran);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("не может быть отрицательной")), Is.True);
    }
}

[TestFixture]
public class ImportSettingsTests
{
    [Test]
    public void Create_BatchSizeTooLarge_Fail()
    {
        // Подготовка
        var settings = new ImportSettings 
        { 
            SourceType = ImportSourceType.Csv, 
            Description = "Test settings",
            StartPosition = 1,
            BatchSize = 10001 
        };

        // Действие
        var results = ValidationHelper.Validate(settings);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Размер пакета")), Is.True);
    }

    [Test]
    public void Create_BatchSizeNegative_Fail()
    {
        // Подготовка
        var settings = new ImportSettings 
        { 
            SourceType = ImportSourceType.Csv, 
            Description = "Test settings",
            StartPosition = 1,
            BatchSize = -5 
        };

        // Действие
        var results = ValidationHelper.Validate(settings);

        // Проверка
        Assert.That(results.Any(r => r.ErrorMessage!.Contains("Размер пакета")), Is.True);
    }
}