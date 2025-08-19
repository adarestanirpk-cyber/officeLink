using Application.Interfaces;
using Application.Services;
using Infrastructure.CrossCutting;
using Infrastructure.FrontOffice.HttpClients;
using Infrastructure.FrontOffice.Persistence;
using Infrastructure.FrontOffice.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.FrontOffice;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontOfficeInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FrontOfficeDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("FrontOfficeConnection")));

        services.AddDbContext<FrontOfficeOutboxDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("FrontOfficeConnection")));

        services.AddScoped<IWFCaseRepository, FrontOfficeWFCaseRepository>();
        services.AddHttpContextAccessor();
        // Cross-cutting services
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        services.AddScoped<IWFCaseLinkService, WFCaseLinkService>();

        // Outbox Service
        services.AddScoped<IOutboxService, FrontOfficeOutboxService>();

        services.AddHttpClient<IBackOfficeClient, BackOfficeClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["BackOfficeApi:BaseUrl"]!);
        });

        return services;
    }
}
