using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Api.Extensions;
using PersonalAccount.Common.Core;
using PersonalAccount.Common.Models;
using PersonalAccount.Console.Extensions;
using PersonalAccount.Data;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Domain.Models;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.IntegrationTests;

[TestFixture]
public class RepositoryTests
{
    private IServiceProvider _provider;

    [OneTimeSetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var services = new ServiceCollection();
        
        // Регистрация всех слоев
        services.RegistryPersonalAccountData(configuration);
        services.RegistryPersonalAccountConsole(configuration);
        services.RegistryPersonalAccountApi(configuration);

        _provider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Простой тест для замера производительности чтения из MS SQL.
    /// </summary>
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task GetRows_JournalRepository_Any(int rows)
    {
        // Подготовка
        var options = _provider.GetRequiredService<ConsoleOptions>();
        var repo = _provider.GetRequiredService<IClientRepository<JournalRowDto>>();
        using var connect = new SqlConnection(options.ConnectionString);

        // Действие
        var result = await repo.GetRows(connect, new LoadingSettingsModel { BatchSize = rows, StartPosition = 0 });

        // Проверки
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Any(), Is.True);
    }

    /// <summary>
    /// Проверить работу высокоскоростной вставки в PostgreSQL.
    /// </summary>
    [Test]
    public async Task SaveRows_JournalRepository_DoesNotThrow()
    {
        // Подготовка
        var repo = _provider.GetRequiredService<IServerRepository<JournalRowDto>>();
        var context = _provider.GetRequiredService<PersonalAccountContext>();
        var connect = context.Database.GetDbConnection();

        var transactions = new List<JournalRowDto>
        {
            new JournalRowDto
            {
                TypeCode = 101, Code = DateTime.Now.Ticks, ReceiptNumber = 1,
                Period = DateTime.UtcNow, Price = 10, Quantity = 1, ProductName = "Test"
            }
        };

        // Создаем иерархию для обхода ограничений NOT NULL в БД
        var company = new CompanyModel { Id = new Guid("14e54725-0efc-42b8-a27d-a84f9a7257c5"), Name = "Test" };
        var branch = new BranchModel { Id = Guid.NewGuid(), Name = "Test Branch", Owner = company };

        var options = new LoadingSettingsModel()
        {
            Owner = branch,
            StartPosition = 1,
            BatchSize = 100
        };

        // Действие и проверка
        Assert.DoesNotThrowAsync(async () => await repo.SaveRows(connect, transactions, options));
    }
}