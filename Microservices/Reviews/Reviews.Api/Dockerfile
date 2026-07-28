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
COPY ["Reviews/Reviews.Api/Reviews.Api.csproj", "Reviews/Reviews.Api/"]
COPY ["Reviews/Reviews.Infrastructure/Reviews.Infrastructure.csproj", "Reviews/Reviews.Infrastructure/"]
COPY ["Reviews/Reviews.Application/Reviews.Application.csproj", "Reviews/Reviews.Application/"]
COPY ["Reviews/Reviews.Domain/Reviews.Domain.csproj", "Reviews/Reviews.Domain/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Reviews/Reviews.Api/Reviews.Api.csproj"
COPY . .
WORKDIR "/src/Reviews/Reviews.Api"
RUN dotnet build "Reviews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Reviews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reviews.Api.dll"]