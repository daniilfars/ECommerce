using Microsoft.Extensions.DependencyInjection;

namespace Modules.Basket.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBasketApplication(this IServiceCollection services)
    {
        // Пока пусто, MediatR добавляется в самом Host

        return services;
    }
}