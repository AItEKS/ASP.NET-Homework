using System.Reflection;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonalAccount.Api.Models;
using PersonalAccount.Api.Logics;
using PersonalAccount.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection(nameof(ApiOptions)));

var apiOptions = builder.Configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>() 
                 ?? throw new InvalidOperationException("Секция ApiOptions не найдена в конфиге!");

var connectionString = apiOptions.PostgreSqlConnectionString;

var upgrader = DeployChanges.To
    .PostgresqlDatabase(connectionString)
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

builder.Services.RegistryPersonalAccountData(connectionString);
builder.Services.RegistryPersonalAccountApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.WebHost.UseUrls("http://0.0.0.0:8000");

// Web приложение
var application = builder.Build();

if (application.Environment.IsDevelopment())
{
    application.UseDeveloperExceptionPage();
}

application.UseSwagger();
application.UseSwaggerUI();

application.UseRouting();
application.MapControllers();

// Запуск
application.Run();