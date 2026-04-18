using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Data;
using PersonalAccount.Data.Extensions;

namespace PersonalAccount.IntegrationTests;

/*
Имя проверяемого метода
Сценарий, в котором тестируется метод
Ожидаемое поведение при вызове сценария
*/

/// <summary>
/// Набор тестов для проверки работы контекста базы данных.
/// </summary>
public class DbContextTests
{
    // Работа с контейнером
    private IServiceProvider _provider;

    public DbContextTests()
    {
       var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration["ApiOptions:PostgreSqlConnectionString"] 
            ?? throw new InvalidOperationException("Строка не найдена!");

        var services = new ServiceCollection()
                     .RegistryPersonalAccountData( connectionString );

        _provider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Проверить выборку данных. Получить список всех организаций.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task FetchCompanies_PersonalAccountContext_Any()
    {
        // Подготовка
        var context = _provider.GetRequiredService<PersonalAccountContext>();
        
        // Действие
        var result = await context.Companies.ToListAsync(CancellationToken.None);

        // Проверка
        Assert.That(result, Is.Not.Null);
    }
}
