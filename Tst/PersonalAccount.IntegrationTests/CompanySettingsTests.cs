using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Data.Logics;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.IntegrationTests;


/*
Имя проверяемого метода
Сценарий, в котором тестируется метод
Ожидаемое поведение при вызове сценария
*/


public class CompanySettingsTests
{

   // Работа с контейнером
    private IServiceProvider _provider;

    public CompanySettingsTests()
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
    /// Прверить работу метода Load класса <see cref="CompanySettingsRepository"/>
    /// </summary>
    /// <returns></returns>
    [Test]
    [TestCase("14e54725-0efc-42b8-a27d-a84f9a7257c5")]
    [Order(2)]
    public async Task  Load_CompanySettingsRepository_NotThrow(string companyId)
    {
        // Подготова
        var repo = _provider.GetRequiredService<ICompanySettingsRepository>();
        var company = new CompanyModel()
        {
            Id = new Guid( companyId )
        };

        // Проверки и действие
        Assert.DoesNotThrowAsync( async() =>
        {
            var result = await repo.LoadAsync(company, CancellationToken.None);
            Assert.That(result is not null);
        });
    }

    /// <summary>
    /// Проверить работу метода Save класса <see cref="CompanySettingsRepository"/>
    /// </summary>
    /// <returns></returns>
    [Test]
    [TestCase("14e54725-0efc-42b8-a27d-a84f9a7257c5")]
    [Order(1)]
    public async Task Save_CompanySettingsRepository_NotThrow(string companyId)
    {
        // Подготовка
        var repo = _provider.GetRequiredService<ICompanySettingsRepository>();
        var company = new CompanyModel()
        {
            Id = new Guid( companyId )
        };
        var setting = new LoadingSettingsModel()
        {
            Owner = company, BatchSize = 10, StartPosition = 0
        };

        // Действие и проверка
        Assert.DoesNotThrowAsync(async () =>
        {
            await repo.SaveAsync(setting, CancellationToken.None);
            var result = await repo.LoadAsync(company, CancellationToken.None);

            Assert.That(result.StartPosition == setting.StartPosition, Is.True);
        });
    }
}
