using MassTransit;

namespace FrontOfficeAPI.Extensions;

public static class MassTransitExtensions
{
    public static void AddRabbitMqWithConsumers(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("192.168.10.36", 30672, "/", h =>
                {
                    h.Username("ataleb");
                    h.Password("023j0554Ie853J");
                });
            });
        });

    }
}
