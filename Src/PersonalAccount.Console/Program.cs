using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PersonalAccount.Console.Models;
using PersonalAccount.Console.Logics;
using PersonalAccount.Domain;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Создаем контейнер DI
var services = new ServiceCollection();

// Регистрируем настройки
services.Configure<ConsoleOptions>(configuration.GetSection(nameof(ConsoleOptions)));

var consoleOptions = configuration.GetSection(nameof(ConsoleOptions)).Get<ConsoleOptions>()
                     ?? throw new InvalidOperationException("Настройки ConsoleOptions не найдены!");

services.AddTransient<JournalRepository>();

var serviceProvider = services.BuildServiceProvider();

CurrentApplication.ShowLogo();
Console.WriteLine("Запуск консольного клиента...");

var journalRepo = serviceProvider.GetRequiredService<JournalRepository>();

while (true)
{
    await Task.Delay(TimeSpan.FromHours(1));
}