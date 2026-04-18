using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Logics;

namespace PersonalAccount.Data.Extensions;

public static class RegistryExtension
{
    public static IServiceCollection RegistryPersonalAccountData(this IServiceCollection services, string connectionString)
    {
        services.AddScoped<IBranchSettingsRepository, BranchSettingsRepository>();
        
        services.AddScoped<IJournalRowRepository, JournalRowRepository>();

        services.AddDbContext<PersonalAccountContext>(x => 
            x.UseNpgsql(connectionString));

        return services;
    }
}