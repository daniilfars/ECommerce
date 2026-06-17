using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Basket.Domain;

namespace Basket.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBasketInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {;
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        services.AddScoped<IBasketRepository, RedisBasketRepository>();

        return services;
    }
}
