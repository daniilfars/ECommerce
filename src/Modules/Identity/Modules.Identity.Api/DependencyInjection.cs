using Microsoft.Extensions.DependencyInjection;

namespace Modules.Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        return services;
    }
}