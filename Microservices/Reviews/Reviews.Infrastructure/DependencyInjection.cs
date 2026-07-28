using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using OrderingGrpc;
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
