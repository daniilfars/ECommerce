# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Host/Host.csproj", "src/Host/"]
COPY ["src/Modules/Basket/Modules.Basket.Api/Modules.Basket.Api.csproj", "src/Modules/Basket/Modules.Basket.Api/"]
COPY ["src/Modules/Basket/Modules.Basket.Application/Modules.Basket.Application.csproj", "src/Modules/Basket/Modules.Basket.Application/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["src/Modules/Catalog/Modules.Catalog.Application/Modules.Catalog.Application.csproj", "src/Modules/Catalog/Modules.Catalog.Application/"]
COPY ["src/Modules/Catalog/Modules.Catalog.Domain/Modules.Catalog.Domain.csproj", "src/Modules/Catalog/Modules.Catalog.Domain/"]
COPY ["src/Modules/Ordering/Modules.Ordering.Application/Modules.Ordering.Application.csproj", "src/Modules/Ordering/Modules.Ordering.Application/"]
COPY ["src/Modules/Ordering/Modules.Ordering.Domain/Modules.Ordering.Domain.csproj", "src/Modules/Ordering/Modules.Ordering.Domain/"]
COPY ["src/Modules/Basket/Modules.Basket.Domain/Modules.Basket.Domain.csproj", "src/Modules/Basket/Modules.Basket.Domain/"]
COPY ["src/Modules/Basket/Modules.Basket.Infrastructure/Modules.Basket.Infrastructure.csproj", "src/Modules/Basket/Modules.Basket.Infrastructure/"]
COPY ["src/Modules/Catalog/Modules.Catalog.Api/Modules.Catalog.Api.csproj", "src/Modules/Catalog/Modules.Catalog.Api/"]
COPY ["src/Modules/Catalog/Modules.Catalog.Infrastructure/Modules.Catalog.Infrastructure.csproj", "src/Modules/Catalog/Modules.Catalog.Infrastructure/"]
COPY ["src/Modules/Identity/Modules.Identity.Api/Modules.Identity.Api.csproj", "src/Modules/Identity/Modules.Identity.Api/"]
COPY ["src/Modules/Identity/Modules.Identity.Application/Modules.Identity.Application.csproj", "src/Modules/Identity/Modules.Identity.Application/"]
COPY ["src/Modules/Identity/Modules.Identity.Domain/Modules.Identity.Domain.csproj", "src/Modules/Identity/Modules.Identity.Domain/"]
COPY ["src/Modules/Identity/Modules.Identity.Infrastructure/Modules.Identity.Infrastructure.csproj", "src/Modules/Identity/Modules.Identity.Infrastructure/"]
COPY ["src/Modules/Ordering/Modules.Ordering.Api/Modules.Ordering.Api.csproj", "src/Modules/Ordering/Modules.Ordering.Api/"]
COPY ["src/Modules/Ordering/Modules.Ordering.Infrastructure/Modules.Ordering.Infrastructure.csproj", "src/Modules/Ordering/Modules.Ordering.Infrastructure/"]
RUN dotnet restore "./src/Host/Host.csproj"
COPY . .
WORKDIR "/src/src/Host"
RUN dotnet build "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.dll"]