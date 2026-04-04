using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;
using PersonalAccount.Data.Logics;
using PersonalAccount.Domain.Core;

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
        services.AddSingleton< ILoadingSettingsRepo, LoadingSettingsRepo>();

        var connectionString = "User ID=admin;Password=123456789;Host=localhost;Port=5433;Database=personal_account;";
        services.AddDbContext<PersonalAccountContext>(
            x => x.UseNpgsql( connectionString )
        );

        return services;
    }
}
