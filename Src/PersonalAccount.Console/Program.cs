using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonalAccount.Common.Core;
using PersonalAccount.Console.Logics;
using PersonalAccount.Console.Models;
using PersonalAccount.Domain.Models.Dto;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<ConsoleOptions>(builder.Configuration.GetSection("ConsoleOptions"));

builder.Services.AddTransient<IClientRepository<JournalRowDto>, JournalRepository>();

builder.Services.AddTransient<ApiClient>();

builder.Services.AddHttpClient<ApiClient>(client =>
{
    var url = builder.Configuration.GetSection("ConsoleOptions:ApiUrl").Value ?? "http://localhost:8000/";
    client.BaseAddress = new Uri(url);
});

builder.Services.AddHostedService<BackgroungPushService>();

using IHost host = builder.Build();

Console.WriteLine("=== КОНСОЛЬНЫЙ КЛИЕНТ ЗАПУЩЕН В ФОНОВОМ РЕЖИМЕ ===");
await host.RunAsync();