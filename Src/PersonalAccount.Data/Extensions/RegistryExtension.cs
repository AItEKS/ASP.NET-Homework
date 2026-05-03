using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;
using PersonalAccount.Common.Models;
using PersonalAccount.Data.Logics;

namespace PersonalAccount.Data.Extensions;

/// <summary>
/// Регистрация сервисов модуля в DI
/// </summary>
public static class RegistryExtension
{
    /// <summary>
    /// Зарегистрировать в контейнере сервисы модуля PersonalAccount.Data
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegistryPersonalAccountData
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var options = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>()
                        ?? throw new InvalidOperationException($"Невозможно загрузить настройки из секции {nameof(ApiOptions)}!");

        services.AddSingleton< ICompanySettingsRepository, CompanySettingsRepository>();
        services.AddDbContext<PersonalAccountContext>(
            x => x.UseNpgsql( options.ConnectionString )
        );
        services.AddSingleton( x => options );
        services.AddScoped<IEntityExtractor, EntityExtractor>();
        services.AddScoped<IBusinessDataRepository, BusinessDataRepository>();
        services.AddScoped<IDataTransformerService, DataTransformerService>();
        services.AddScoped<IJournalDataSource, JournalRowDataSource>();

        return services;
    }
}
