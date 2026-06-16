using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Identity.Application.Interfaces;
using Identity.Domain;
using Identity.Infrastructure.Configurations;
using Identity.Infrastructure.Data;
using Identity.Infrastructure.Services;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("JwtSettings");
        var jwtSettings = jwtSection.Get<JwtSettings>();

        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            jwtSettings = new JwtSettings
            {
                SecretKey = "A_Very_Long_Secret_Key_For_Testing_Purposes_32_Chars_Minimum",
                Issuer = "Shop.Api",
                Audience = "Shop.Client",
                ExpiryMinutes = 15
            };
        }

        services.AddSingleton(jwtSettings);

        services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppIdentityDbContext>(sp =>
            sp.GetRequiredService<AppIdentityDbContext>());

        services.AddIdentity<User, IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AppIdentityDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}