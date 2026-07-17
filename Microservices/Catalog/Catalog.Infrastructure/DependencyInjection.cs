using Catalog.Application.Consumers;
using Catalog.Application.Interfaces;
using Catalog.Application.Models;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Services;
using Elastic.Clients.Elasticsearch;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using StackExchange.Redis;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticUri = configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
        var settings = new ElasticsearchClientSettings(new Uri(elasticUri))
            .DefaultIndex("products");

        var elasticClient = new ElasticsearchClient(settings);

        var indexExistsResponse = elasticClient.Indices.Exists("products");
        if (!indexExistsResponse.Exists)
        {
            elasticClient.Indices.Create("products", c => c
                .Settings(s => s
                    .Analysis(a => a
                        .TokenFilters(tf => tf
                            .Stemmer("english_stemmer", st => st.Language("english"))
                            .Stemmer("russian_stemmer", st => st.Language("russian"))
                        )
                        .Analyzers(an => an
                            .Custom("ru_en_analyzer", ca => ca
                                .Tokenizer("standard")
                                .Filter(new[] {
                            "lowercase",
                            "english_stemmer",
                            "russian_stemmer"
                                })
                            )
                        )
                    )
                )
                .Mappings(m => m
                    .Properties<ProductSearchDocument>(p => p
                        .Text(f => f.Name, t => t.Analyzer("ru_en_analyzer"))
                    )
                )
            );
        }


        services.AddSingleton(elasticClient);

        var redisConnectionString = configuration.GetConnectionString("Redis")!;

        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.AddSingleton<IDatabase>(sp => multiplexer.GetDatabase());

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () => Task.FromResult<IConnectionMultiplexer>(multiplexer);
        });

        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICatalogDbContext>(sp =>
            sp.GetRequiredService<CatalogDbContext>());

        services.AddSingleton<ICatalogCacheService, CatalogCacheService>();

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

        services.AddMassTransit(x =>
        {
            x.AddConsumer<StockReserveRequestedConsumer>();
            x.AddConsumer<OrderCancelledConsumer>();
            x.AddConsumer<ProductCreatedConsumer>();
            x.AddConsumer<ProductUpdatedConsumer>();
            x.AddConsumer<ProductDeletedConsumer>();

            x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
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