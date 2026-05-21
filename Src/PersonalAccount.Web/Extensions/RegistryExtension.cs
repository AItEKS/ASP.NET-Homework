using System;
using PersonalAccount.Api.Logics;
using PersonalAccount.Common.Core;
using PersonalAccount.Web.Logics;

namespace PersonalAccount.Web.Extensions;

public static class RegistryExtension
{
    /// <summary>
    /// Зарегистрировать в контейнере сервисы модуля PersonalAccount.Web
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection RegistryPersonalAccountWeb
    (
        this IServiceCollection services,
          IConfiguration configuration
    )
    {
        services.AddScoped<IBranchRepository, BranchRepository>();

        // Сервисы построения отчетов
        services.AddScoped<IRevenueReportService, RevenueReportService>();
        services.AddScoped<ISalesReportService, SalesReportService>();
        services.AddScoped<IWorkScheduleReportService, WorkScheduleReportService>();

        // Сервис-фабрика отчетов
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
