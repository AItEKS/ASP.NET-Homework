using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PersonalAccount.Data.Logics;
using PersonalAccount.Domain.Models;

namespace PersonalAccount.UnitTests;

[TestFixture]
public class LoadingSettingsTests
{   
    private string _connectionString;
    
    [OneTimeSetUp] 
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var configuration = builder.Build();
        
        _connectionString = configuration["ConnectionString"] ?? throw new InvalidOperationException("Строка подключения не найдена");
    }

    [Test]
    public async Task Save_LoadingSettingsRepo_OK()
    {
        var repo = new LoadingSettingsRepo(_connectionString);
        
        var org = new Organization()
        {
            Id = new Guid("11111111-1111-1111-1111-111111111101"),
            Name = "ООО Ромашка",
            Inn = "7701234567",
            Address = "г. Москва, ул. Ленина, д. 10",
        };

        org.Settings = new ImportSettings
        {
            StartPosition = 123,
            SourceType = ImportSourceType.Csv,
            BatchSize = 100,
            Description = "Test"
        };

        bool saveResult = await repo.Save(org, CancellationToken.None);

        Assert.That(saveResult, Is.True);
    }

    [Test]
    public async Task Load_LoadingSettingsRepo_OK()
    {
        var repo = new LoadingSettingsRepo(_connectionString);
        
        var org = new Organization()
        {
            Id = new Guid("11111111-1111-1111-1111-111111111101"),
            Name = "ООО Ромашка",
            Inn = "7701234567",
            Address = "г. Москва, ул. Ленина, д. 10",
        };

        bool loadResult = await repo.Load(org, CancellationToken.None);

        Assert.That(loadResult, Is.True);
    }
}