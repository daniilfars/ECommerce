using Microsoft.Extensions.DependencyInjection;

namespace Modules.Ordering.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderingApplication(this IServiceCollection services)
    {
        // Пока пусто, MediatR добавляется в самом Host

        return services;
    }
}
