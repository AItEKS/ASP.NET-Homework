using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Common.Core;
using PersonalAccount.Data;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalAccount.IntegrationTests;

[TestFixture]
public class CompanySettingsTests
{
    private IServiceProvider _provider = null!;
    private readonly Guid _testBranchId = new Guid("14e54725-0efc-42b8-a27d-a84f9a7257c5");

    [OneTimeSetUp]
    public void Setup()
    {
       var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

        var configuration = builder.Build();
        var services = new ServiceCollection().RegistryPersonalAccountData(configuration);
        _provider = services.BuildServiceProvider();

        // Гарантируем наличие тестового филиала в базе
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PersonalAccountContext>();
        
        var company = context.Companies.FirstOrDefault() ?? new PersonalAccount.Data.Models.Company 
        { 
            Id = Guid.NewGuid(), Name = "Test", Inn = "1234567890", Address = "г. Москва, ул. Ленина, д. 1" 
        };
        if (context.Entry(company).State == EntityState.Detached) context.Companies.Add(company);

        if (!context.Branches.Any(x => x.Id == _testBranchId))
        {
            context.Branches.Add(new PersonalAccount.Data.Models.Branch 
            { 
                Id = _testBranchId, 
                Name = "Test Branch", 
                CompanyId = company.Id 
            });
            context.SaveChanges();
        }
    }

    [Test]
    [Order(2)]
    public void Load_BranchSettingsRepository_NotThrow()
    {
        var repo = _provider.GetRequiredService<IBranchSettingsRepository>();
        var branch = new BranchModel { Id = _testBranchId, Name = "branch" };

        Assert.DoesNotThrow(() => {
            repo.Load(branch);
        });
    }

    [Test]
    [Order(1)]
    public void Save_BranchSettingsRepository_NotThrow()
    {
        var repo = _provider.GetRequiredService<IBranchSettingsRepository>();
        var branch = new BranchModel { Id = _testBranchId, Name = "branch" };
        var setting = new LoadingSettingsModel { Owner = branch, BatchSize = 10, StartPosition = 0 };

        Assert.DoesNotThrow(() => {
            repo.Save(setting);
            var result = repo.Load(branch);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.StartPosition, Is.EqualTo(setting.StartPosition));
        });
    }
}