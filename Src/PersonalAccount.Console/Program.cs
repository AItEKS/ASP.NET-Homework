using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PersonalAccount.Console.Extensions;
using PersonalAccount.Domain;
using Serilog;

// Лого
CurrentApplication.ShowLogo();

// Логгер
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File(
                    path: "PersonalAccountConsole_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
                .CreateLogger();                 

// Настройки приложения и сервисы
var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

// Создали хост
var hostBuilder = Host.CreateDefaultBuilder()
          .ConfigureServices((context, services) =>
          {
            services.RegistryPersonalAccountConsole(configuration);
          });

var host = hostBuilder.Build(); 

await host.RunAsync();