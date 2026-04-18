using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using PersonalAccount.Console.Logics;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.IntegrationTests;

/// <summary>
/// Набор интеграционных тестов для проверки работы различных репозиториев.
/// </summary>
public class RepositoryTests
{
    private IServiceProvider _provider;

    public RepositoryTests()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        var configuration = builder.Build();

        var services = new ServiceCollection();

        services.Configure<ConsoleOptions>(configuration.GetSection(nameof(ConsoleOptions)));

        services.AddTransient<JournalRepository>();

        _provider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Простой тест для замера производительности.
    /// </summary>
    [Test]
    [TestCase(100)][TestCase(1000)]
    [TestCase(10000)]
    public async Task GetRows_JournalRepository_Fetch(int rows)
    {
        var repo = _provider.GetRequiredService<JournalRepository>();
        var options = _provider.GetRequiredService<IOptions<ConsoleOptions>>().Value;

        Assert.That(string.IsNullOrEmpty(options.MsSqlConnectionString), Is.False, "Строка MS SQL не загружена из JSON!");

        // Подготовка
        using var connect = new SqlConnection(options.MsSqlConnectionString);
        
        await connect.OpenAsync();

        var settings = new LoadingSettingsModel() { BatchSize = rows };

        // Действие
        var result = await repo.GetRows(connect, settings);

        // Проверки
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Any(), Is.True);
        
        Assert.That(result.Count(), Is.LessThanOrEqualTo(rows));
    }
}