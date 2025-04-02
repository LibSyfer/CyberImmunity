# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Greenhouse.TomatoDbConnectionModule/Greenhouse.TomatoDbConnectionModule.csproj", "Greenhouse.TomatoDbConnectionModule/"]
COPY ["Greenhouse.MessageBus.RabbitMQ/Greenhouse.MessageBus.RabbitMQ.csproj", "Greenhouse.MessageBus.RabbitMQ/"]
COPY ["Greenhouse.MessageBus/Greenhouse.MessageBus.csproj", "Greenhouse.MessageBus/"]
COPY ["Greenhouse.Share/Greenhouse.Share.csproj", "Greenhouse.Share/"]
RUN dotnet restore "./Greenhouse.TomatoDbConnectionModule/Greenhouse.TomatoDbConnectionModule.csproj"
COPY . .
WORKDIR "/src/Greenhouse.TomatoDbConnectionModule"
RUN dotnet build "./Greenhouse.TomatoDbConnectionModule.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Greenhouse.TomatoDbConnectionModule.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Greenhouse.TomatoDbConnectionModule.dll"]