using System;
using System.Collections.Generic;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.Performance.Services;

public static class FakeDataGenerator
{
    public static List<Transaction> GenerateTransactions(int count, DateTime baseDate, Guid orgId)
    {
        var random = new Random(42); 
        
        var transactions = new List<Transaction>(count);

        var fakeOrg = new Organization 
        { 
            Id = orgId, 
            Name = "ООО Бенчмарк", 
            Inn = "7700000000", 
            Address = "Серверная" 
        };

        var fakeEmployee = new Employee 
        { 
            Id = Guid.NewGuid(), 
            Name = "Тестовый Сотрудник",
            Organization = new List<Organization> { fakeOrg }
        };

        var fakeCategory = new Category 
        { 
            Id = Guid.NewGuid(), 
            Name = "Тестовая Категория" 
        };

        var fakeProduct = new Nomenclature 
        { 
            Id = Guid.NewGuid(), 
            Name = "Тестовый Товар", 
            Category = fakeCategory,
            Price = 1000m,
            UnitOfMeasure = "шт"
        };

        var operationTypes = new[] 
        { 
            OperationType.Cash, OperationType.Visa, OperationType.Refund, 
            OperationType.PluSales, OperationType.StartWork, OperationType.EndWork 
        };

        for (int i = 0; i < count; i++)
        {
            var randomDaysOffset = random.Next(-15, 15);
            var randomDate = new DateTimeOffset(baseDate.AddDays(randomDaysOffset), TimeSpan.Zero);

            var opType = operationTypes[random.Next(operationTypes.Length)];

            transactions.Add(new Transaction
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                Employee = fakeEmployee,
                Nomenclature = fakeProduct,
                OperationType = opType,
                
                Amount = (decimal)random.Next(100, 5000),
                Quantity = (decimal)random.Next(1, 10),
                Discount = (decimal)random.Next(0, 100),
                
                OperationDate = randomDate,
                CreatedAt = DateTime.UtcNow
            });
        }

        return transactions;
    }
}