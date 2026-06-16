using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        // Пока пусто, MediatR добавляется в самом Host

        return services;
    }
}