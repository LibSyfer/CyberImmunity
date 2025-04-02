# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Greenhouse.CoordinatorModule/Greenhouse.CoordinatorModule.csproj", "Greenhouse.CoordinatorModule/"]
COPY ["Greenhouse.MessageBus.RabbitMQ/Greenhouse.MessageBus.RabbitMQ.csproj", "Greenhouse.MessageBus.RabbitMQ/"]
COPY ["Greenhouse.MessageBus/Greenhouse.MessageBus.csproj", "Greenhouse.MessageBus/"]
COPY ["Greenhouse.Share/Greenhouse.Share.csproj", "Greenhouse.Share/"]
RUN dotnet restore "./Greenhouse.CoordinatorModule/Greenhouse.CoordinatorModule.csproj"
COPY . .
WORKDIR "/src/Greenhouse.CoordinatorModule"
RUN dotnet build "./Greenhouse.CoordinatorModule.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Greenhouse.CoordinatorModule.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Greenhouse.CoordinatorModule.dll"]