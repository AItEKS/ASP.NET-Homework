using Microsoft.Extensions.Configuration;
using PersonalAccount.Console.Models;
using DbUp;
using System.Reflection;

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

var configuration = builder.Build();
var options = configuration.Get<ApplicationOptions>() ?? throw new InvalidOperationException("Не удалось прочитать appsettings.json");

// Подключаем миграцию
var upgrader =  DeployChanges.To
            .PostgresqlDatabase(options.ConnectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetAssembly(typeof(PersonalAccount.Data.PersonalAccountDataMarker)))
            .LogToConsole()
            .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
}