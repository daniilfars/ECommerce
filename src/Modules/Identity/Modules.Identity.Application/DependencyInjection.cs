using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Application.Commands.Register;

namespace Modules.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly));

        return services;
    }
}