using Microsoft.Extensions.Configuration;
using PersonalAccount.Console.Models;
using PersonalAccount.Data;

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

var configuration = builder.Build();
var options = configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException("Не удалось прочитать appsettings.json");

var initializer = new DatabaseInitializer(options.ConnectionString);

initializer.Init();
initializer.SeedData();
