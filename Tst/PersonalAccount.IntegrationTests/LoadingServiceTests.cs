using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Api.Logics;
using PersonalAccount.Common.Core;
using PersonalAccount.Data;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.IntegrationTests;

[TestFixture]
public class LoadingServiceTests
{
    private IServiceProvider _provider = null!; 
    
    private readonly string _branchIdStr = "14e54725-0efc-42b8-a27d-a84f9a7257c5";

    [OneTimeSetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration["ApiOptions:PostgreSqlConnectionString"]
            ?? throw new InvalidOperationException("Строка подключения не найдена");

        var services = new ServiceCollection();
        
        services.RegistryPersonalAccountData(connectionString);

        services.AddScoped<ILoadingService, LoadingService>();

        _provider = services.BuildServiceProvider();
    }

    [Test][Order(1)]
    public async Task PushAsync_ValidTransactions_ShouldInsertAndAdvanceSettings()
    {
        // Подготовка
        var loadingService = _provider.GetRequiredService<ILoadingService>();
        
        var settingsRepo = _provider.GetRequiredService<IBranchSettingsRepository>();
        
        var dbContext = _provider.GetRequiredService<PersonalAccountContext>();

        var branch = new BranchModel 
        { 
            Id = new Guid(_branchIdStr),
            Name = "Тестовый филиал"
        };

        var currentSettings = await settingsRepo.LoadAsync(branch, CancellationToken.None);
        long startPos = currentSettings?.StartPosition ?? 1;

        long newCode1 = startPos + 100; 
        long newCode2 = startPos + 101;

        var transactions = new List<JournalRowDto>
        {
            new JournalRowDto
            {
                Code = newCode1,
                TypeCode = 101,
                ReceiptNumber = 777,
                Period = DateTime.UtcNow,
                Quantity = 1,
                Price = 150.50,
                Discount = 0,
                EmploeeName = "Тест Сотрудник 1",
                NomenclatureName = "Интеграционный Товар 1"
            },
            new JournalRowDto
            {
                Code = newCode2,
                TypeCode = 101,
                ReceiptNumber = 777,
                Period = DateTime.UtcNow,
                Quantity = 2,
                Price = 300,
                Discount = 10,
                EmploeeName = "Тест Сотрудник 1",
                NomenclatureName = "Интеграционный Товар 2"
            }
        };

        // Действие
        var isSuccess = await loadingService.PushAsync(branch, transactions, CancellationToken.None);

        // Проверка
        Assert.That(isSuccess, Is.True, "Метод PushAsync должен вернуть true");

        var updatedSettings = await settingsRepo.LoadAsync(branch, CancellationToken.None);
        Assert.That(updatedSettings, Is.Not.Null, "Настройки филиала не должны быть null");
        Assert.That(updatedSettings.StartPosition, Is.GreaterThanOrEqualTo(startPos), "StartPosition должен сдвинуться");

        dbContext.ChangeTracker.Clear();

        // Проверяем
        var savedRow = await dbContext.JournalRows
            .AsNoTracking()
            .OrderByDescending(r => r.Period)
            .FirstOrDefaultAsync(r => r.NomenclatureName == "Интеграционный Товар 1");

        Assert.That(savedRow, Is.Not.Null, "Строка журнала должна быть сохранена в БД");
        Assert.That(savedRow!.NomenclatureName, Is.EqualTo("Интеграционный Товар 1"));
    }
}