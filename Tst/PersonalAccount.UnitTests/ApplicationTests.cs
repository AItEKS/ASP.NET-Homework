using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using PersonalAccount.Domain;
using PersonalAccount.Domain.Core;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.UnitTests;

[TestFixture]
public class ApplicationTests
{
    [Test]
    public void Create_Transaction_FalseValidate()
    {
        var transaction = new TransactionModel()
        {
            Emploee = new EmploeeModel() { Name = "test" },
            Owner = new CompanyModel() { Name = "test" },
            Type = TransactionType.Sale,
            Quantuty = 1, Price = 1, 
            Period = DateTimeOffset.Now,
            Nomenclature = new NomenclatureModel() { Name = "Test" }
        };

        // Действие
        var result = transaction.Validate();

        // Проверка
        Assert.That(result, Is.False);
    }

    [Test]
    public void Create_Transaction_TrueValidate()
    {
        // Подготовка
        var company = new CompanyModel 
        { 
            Name = "test", INN = "1234567890", 
            Address = "90000, г. Москва, ул. Советская, д. 12" 
        };
        var branch = new BranchModel { Name = "Branch", Owner = company };

        var transaction = new TransactionModel()
        {
            Id = Guid.NewGuid(),
            Owner = company,
            Branch = branch,
            Emploee = new EmploeeModel { Name = "test", Phone = "+79041518166", Owner = company },
            Type = TransactionType.Sale,
            Quantuty = 1, Price = 1, 
            Period = DateTimeOffset.Now,
            Nomenclature = new NomenclatureModel 
            { 
                Name = "Test", 
                Category = new CategoryModel { Name = "Test", Owner = company }
            }
        };

        // Действие
        var result = transaction.Validate();

        // Проверка
        Assert.That(result, Is.True);
        Assert.That(transaction.IsError, Is.False);
    }

    [Test]
    public void CurrentVersion_Show_Any()
    {
        var version = CurrentApplication.CurrentVersion();
        Assert.That(string.IsNullOrEmpty(version), Is.False);
    }
}