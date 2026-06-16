using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Catalog.Application.Interfaces;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICatalogDbContext>(sp =>
            sp.GetRequiredService<CatalogDbContext>());

        var minioSettings = configuration.GetSection("Minio");
        var endpoint = minioSettings["Endpoint"]!;
        var accessKey = minioSettings["AccessKey"];
        var secretKey = minioSettings["SecretKey"];
        var bucketName = minioSettings["BucketName"]!;

        services.AddSingleton<IMinioClient>(m => 
            new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build());

        services.AddScoped<IImageStorageService>(sp =>
            new ImageStorageService(
                sp.GetRequiredService<IMinioClient>(),
                bucketName,
                endpoint));

        return services;
    }
}