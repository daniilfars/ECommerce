using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.Basket.Api;
using Modules.Basket.Application;
using Modules.Basket.Infrastructure;
using Modules.Catalog.Api;
using Modules.Catalog.Application;
using Modules.Catalog.Infrastructure;
using Modules.Catalog.Infrastructure.Data;
using Modules.Identity.Api;
using Modules.Identity.Application;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure;
using Modules.Identity.Infrastructure.Configurations;
using Modules.Identity.Infrastructure.Data;
using Modules.Ordering.Api;
using Modules.Ordering.Application;
using Modules.Ordering.Infrastructure;
using Modules.Ordering.Infrastructure.Data;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)); // Для SQL логов LogEventLevel.Information

// Подрубаем настройки от модуля identity
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddIdentityApi();

// Подрубаем настройки от модуля catalog
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddCatalogApplication();
builder.Services.AddCatalogApi();

// Подрубаем настройки от модуля basket
builder.Services.AddBasketInfrastructure(builder.Configuration);
builder.Services.AddBasketApplication();
builder.Services.AddBasketApi();

// Подрубаем настройки от модуля ordering
builder.Services.AddOrderingInfrastructure(builder.Configuration);
builder.Services.AddOrderingApplication();
builder.Services.AddOrderingApi();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Modules.Identity.Application.Commands.Register.RegisterUserHandler).Assembly,
    typeof(Modules.Catalog.Application.Commands.CreateProduct.CreateProductHandler).Assembly,
    typeof(Modules.Basket.Application.Commands.AddItemToBasket.AddItemToBasketHandler).Assembly,
    typeof(Modules.Ordering.Application.Commands.CreateOrder.CreateOrderHandler).Assembly
));

// Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = ClaimTypes.Role
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope()) //Применение миграций для докера
{
    try
    {
        app.Logger.LogInformation("Starting Identity migration...");
        var identityContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        identityContext.Database.Migrate();
        app.Logger.LogInformation("Identity migration completed.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Identity migration failed");
        throw;
    }

    try
    {
        app.Logger.LogInformation("Starting Catalog migration...");
        var catalogContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        catalogContext.Database.Migrate();
        app.Logger.LogInformation("Catalog migration completed.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Catalog migration failed");
        throw;
    }

    try
    {
        app.Logger.LogInformation("Starting Ordering migration...");
        var orderingContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        orderingContext.Database.Migrate();
        app.Logger.LogInformation("Ordering migration completed.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Ordering migration failed");
        throw;
    }
}

await AddAdminAsync(app);

app.Run();

async Task AddAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));

    var adminEmail = "admin@shop.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser is null)
    {
        var admin = User.Create("Admin", "Admin", adminEmail).Value!;
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}