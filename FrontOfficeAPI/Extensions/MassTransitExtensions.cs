using Application.Messaging.Consumers;
using Domain.ValueObjects;
using MassTransit;

namespace FrontOfficeAPI.Extensions;

public static class MassTransitExtensions
{
    public static void AddRabbitMqWithConsumers(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection("RabbitMq").Get<RabbitMqSettings>();
        var uri = new Uri($"rabbitmq://{rabbitConfig.Host}:{rabbitConfig.Port}{rabbitConfig.VirtualHost}");

        services.AddMassTransit(x =>
        {
            x.AddConsumer<WFCaseLinkConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(uri, h =>
                {
                    h.Username(rabbitConfig.Username);
                    h.Password(rabbitConfig.Password);
                });

                //cfg.ReceiveEndpoint("frontoffice-wfcaselink-queue", e =>
                //{
                //    e.ConfigureConsumer<WFCaseLinkConsumer>(context);
                //    //one minute to send
                //    e.SetQueueArgument("x-message-ttl", 60000);
                //});
            });
        });
    }
}
