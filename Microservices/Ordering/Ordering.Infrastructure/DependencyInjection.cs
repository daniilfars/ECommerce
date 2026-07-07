using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Consumers;
using Ordering.Application.Interfaces;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderingDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IOrderingDbContext>(sp =>
            sp.GetRequiredService<OrderingDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.AddConsumer<StockReserveFailedConsumer>();
            x.AddConsumer<StockReservedConsumer>();

            x.AddEntityFrameworkOutbox<OrderingDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "rabbitmq", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });

                cfg.UseMessageRetry(r => r.Exponential(
                    4,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(3)
                ));

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}