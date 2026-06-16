using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogApplication(this IServiceCollection services)
    {
        // Пока пусто, MediatR добавляется в самом Host

        return services;
    }
}