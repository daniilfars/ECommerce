using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderingGrpc;
using Reviews.Application.Consumers;
using Reviews.Application.Interfaces;
using Reviews.Infrastructure.Data;

namespace Reviews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddReviewsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IReviewsDbContext>(sp =>
            sp.GetRequiredService<ReviewsDbContext>());

        services.AddDbContext<ReviewsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ProductDeletedConsumer>();

            x.AddEntityFrameworkOutbox<ReviewsDbContext>(o =>
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

        services.AddGrpcClient<OrderingService.OrderingServiceClient>(options =>
        {
            options.Address = new Uri(configuration["GrpcSettings:OrderingUrl"] ?? "http://ordering-api:5001");
        })
        .ConfigureChannel(options =>
        {
            options.UnsafeUseInsecureChannelCallCredentials = true;
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            EnableMultipleHttp2Connections = true
        })
        .AddCallCredentials((context, metadata, serviceProvider) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                metadata.Add("Authorization", token);
            }
            return Task.CompletedTask;
        });

        return services;
    }
}
