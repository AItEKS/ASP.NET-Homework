using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Extensions;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.IntegrationTests;

[TestFixture]
public class CompanySettingsTests
{
    private IServiceProvider _provider = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        var configuration = builder.Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration["ApiOptions:PostgreSqlConnectionString"] 
            ?? throw new InvalidOperationException("Строка не найдена!");

        var services = new ServiceCollection();
        services.RegistryPersonalAccountData(connectionString);

        _provider = services.BuildServiceProvider();
    }

    [Test]
    [TestCase("14e54725-0efc-42b8-a27d-a84f9a7257c5")]
    [Order(2)]
    public async Task Load_BranchSettingsRepository_NotThrow(string branchId)
    {
        var repo = _provider.GetRequiredService<IBranchSettingsRepository>();
        var branch = new BranchModel()
        {
            Name = "branch",
            Id = new Guid(branchId)
        };

        var result = await repo.LoadAsync(branch, CancellationToken.None);
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    [TestCase("14e54725-0efc-42b8-a27d-a84f9a7257c5")]
    [Order(1)]
    public async Task Save_BranchSettingsRepository_NotThrow(string branchId)
    {
        var repo = _provider.GetRequiredService<IBranchSettingsRepository>();
        var branch = new BranchModel()
        {
            Name = "branch",
            Id = new Guid(branchId)
        };
        var setting = new LoadingSettingsModel()
        {
            Owner = branch, 
            BatchSize = 10, 
            StartPosition = 0
        };

        await repo.SaveAsync(setting, CancellationToken.None);
        var result = await repo.LoadAsync(branch, CancellationToken.None);

        Assert.That(result.StartPosition, Is.EqualTo(setting.StartPosition));
    }
}