using Application.Interfaces;
using Application.Services;
using Infrastructure.BackOffice.HttpClients;
using Infrastructure.BackOffice.Persistence;
using Infrastructure.BackOffice.Services;
using Infrastructure.CrossCutting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence;
namespace Infrastructure.BackOffice;

public static class DependencyInjection
{
    public static IServiceCollection AddBackOfficeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<BackOfficeDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("BackOfficeConnection")));

        services.AddDbContext<BackOfficeOutboxDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("BackOfficeConnection")));

        services.AddScoped<IWFCaseRepository, BackOfficeWFCaseRepository>();

        services.AddHttpContextAccessor();

        // Cross-cutting services
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        services.AddScoped<IWFCaseLinkService, WFCaseLinkService>();

        // Outbox Service
        services.AddScoped<IOutboxService, BackOfficeOutboxService>();

        // 🔹 ثبت BackOfficeClient
        services.AddHttpClient<IFrontOfficeClient, FrontOfficeClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["FrontOfficeApi:BaseUrl"]!);
        });

        return services;
    }
}
