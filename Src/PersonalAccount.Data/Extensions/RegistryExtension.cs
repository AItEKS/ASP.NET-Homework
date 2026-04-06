using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;
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
        string connectionString
    )
    {
        services.AddSingleton< ICompanySettingsRepository, CompanySettingsRepository>();

        services.AddDbContext<PersonalAccountContext>(
            x => x.UseNpgsql( connectionString )
        );

        return services;
    }
}
