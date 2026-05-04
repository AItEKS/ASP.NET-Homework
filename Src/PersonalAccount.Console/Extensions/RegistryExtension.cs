using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;
using PersonalAccount.Common.Models;
using PersonalAccount.Console.Logics;
using PersonalAccount.Domain.Models.Dto;

namespace PersonalAccount.Console.Extensions;

public static class RegistryExtension
{
    public static IServiceCollection RegistryPersonalAccountConsole(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(ConsoleOptions)).Get<ConsoleOptions>()
                        ?? throw new InvalidOperationException($"Невозможно загрузить настройки!");

        services.Configure<ConsoleOptions>(configuration.GetSection("ConsoleOptions"));
        services.AddScoped<IClientRepository<JournalRowDto>, JournalReadRepository>();
        
        services.AddHttpClient<ApiClient>(client => 
        {
            client.BaseAddress = new Uri(options.ServerHost);
        });

        services.AddHostedService<BackgroungPushService>();
        
        return services;
    }
}