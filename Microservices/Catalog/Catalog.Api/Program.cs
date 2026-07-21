using Catalog.Api.Services;
using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

AppContext.SetSwitch("System.Net.SocketsHttpHandler.Http2UnencryptedSupport", true);

builder.Services.AddGrpc();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(Catalog.Application.Commands.CreateProduct.CreateProductHandler).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddCatalogApplication();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!)),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

builder.Services.AddMetricServer(options => { options.Port = 1234; });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpMetrics();
app.MapControllers();
app.MapMetrics();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    context.Database.Migrate();
}

app.MapGrpcService<ProductGrpcService>();

app.Run();