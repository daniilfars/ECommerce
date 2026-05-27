using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Application.Interfaces;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure.Configurations;
using Modules.Identity.Infrastructure.Data;
using Modules.Identity.Infrastructure.Services;

namespace Modules.Identity.Infrastructure;

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
                Audience = "Shop.Api",
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

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = ".AspNetCore.Identity.Application";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                return Task.CompletedTask;
            };
        });

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}