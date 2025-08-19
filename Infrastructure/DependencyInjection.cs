using Application.Interfaces;
using Infrastructure.HttpClients;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFrontOfficeInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddDbContext<FrontOfficeDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("FrontOfficeConnection")));

        services.AddScoped<IWFCaseRepository, FrontOfficeWFCaseRepository>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IClient, Client>();

        return services;
    }

    public static IServiceCollection AddBackOfficeInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddDbContext<BackOfficeDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("BackOfficeConnection")));

        services.AddScoped<IWFCaseRepository, BackOfficeWFCaseRepository>();

        services.AddHttpContextAccessor();
        services.AddHttpClient<IClient, Client>();

        return services;
    }
}
