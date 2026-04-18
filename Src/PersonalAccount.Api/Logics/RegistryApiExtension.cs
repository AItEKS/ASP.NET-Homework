using Microsoft.Extensions.DependencyInjection;
using PersonalAccount.Common.Core;

namespace PersonalAccount.Api.Logics;

public static class RegistryApiExtension
{
    public static IServiceCollection RegistryPersonalAccountApi(this IServiceCollection services)
    {
        services.AddScoped<IRevenueReportService, RevenueReportService>();
        services.AddScoped<ISalesReportService, SalesReportService>();
        services.AddScoped<IWorkScheduleReportService, WorkScheduleReportService>();
        services.AddScoped<ILoadingService, LoadingService>();

        return services;
    }
}