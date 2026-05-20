using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Api.Extensions;
using PersonalAccount.Api.Logics;
using PersonalAccount.Common.Models;
using PersonalAccount.Data.Extensions;
using Serilog;

// Настройки и построитель Web приложения
var builder = WebApplication.CreateBuilder();
var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();


var options = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>()
                        ?? throw new InvalidOperationException($"Невозможно загрузить настройки из секции {nameof(ApiOptions)}!");

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: "PersonalAccountApi_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30
                )
                .CreateLogger();                 

Log.Information("Начало запуска Personal Account Api");

// Миграции
var upgrader =  DeployChanges.To
            .PostgresqlDatabase(  options.ConnectionString )
            .WithScriptsEmbeddedInAssembly(Assembly.GetAssembly(typeof(PersonalAccount.Data.PersonalAccountDataMarker)))
            .LogToConsole()
            .Build();

var result = upgrader.PerformUpgrade();
if (!result.Successful)
{
    Log.Error( result.Error, "Ошибка при миграции данных!");
    // return;
}

var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

// Подключение сервисов
builder.Services
        .RegistryPersonalAccountData( configuration )
        .RegistryPersonalAccountApi (configuration )
        .AddSwaggerGen(x =>
        {
            x.IncludeXmlComments(xmlPath);
        });

// Настройки Web
builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:8000");

// Web приложение
var application = builder.Build();

application.UseDeveloperExceptionPage();
application.UseRouting();
application.MapControllers();
application.UseSwagger();
application.UseSwaggerUI();

application.UseMiddleware<ExceptionMiddleware>();

// Запуск
Log.Information("Приложение Personal Account запущено успешно!");
application.Run();