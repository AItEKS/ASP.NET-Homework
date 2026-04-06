using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using PersonalAccount.Api.Models;
using PersonalAccount.Console.Logics;
using PersonalAccount.Console.Models;

namespace PersonalAccount.IntegrationTests;

/*
Имя проверяемого метода
Сценарий, в котором тестируется метод
Ожидаемое поведение при вызове сценария
*/


/// <summary>
/// Набор интеграционных тестов для проверки работы различных репозиториев.
/// </summary>
public class RepositoryTests
{
    // Настройки текущие
    private ApiOptions _options;

    public RepositoryTests()
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        _options = configuration.Get<ApiOptions>()
                        ?? throw new InvalidOperationException("Unabled loading appsettings.json!");
    }

    /// <summary>
    /// Простой тест для замера производительности.
    /// </summary>
    /// <param name="rows"></param>
    /// <returns></returns>
    [Test]
    [TestCase(100)]
    [TestCase(1000)]
    [TestCase(10000)]
    public async Task GetRows_JournalRepository_Fetch(int rows)
    {
        // Подготовка
        using var connect = new SqlConnection(_options.MsSqlConnectionString);
        var repo = new JournalRepository();

        // Действие
        var result = await repo.GetRows(connect, new Domain.Models.LoadingSettingsModel() { BatchSize = rows });

        // Проверки
        Assert.That(result is not null);
        Assert.That(result!.Any());
        Assert.That(result!.Count() == rows);
    }
}
