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
COPY ["Ordering/Ordering.Api/Ordering.Api.csproj", "Ordering/Ordering.Api/"]
COPY ["Ordering/Ordering.Application/Ordering.Application.csproj", "Ordering/Ordering.Application/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Ordering/Ordering.Domain/Ordering.Domain.csproj", "Ordering/Ordering.Domain/"]
COPY ["Ordering/Ordering.Infrastructure/Ordering.Infrastructure.csproj", "Ordering/Ordering.Infrastructure/"]
RUN dotnet restore "Ordering/Ordering.Api/Ordering.Api.csproj"
COPY . .
WORKDIR "/src/Ordering/Ordering.Api"
RUN dotnet build "Ordering.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ordering.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ordering.Api.dll"]