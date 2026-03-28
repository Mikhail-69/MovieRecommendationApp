FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем всё
COPY . .

# Восстанавливаем зависимости
RUN dotnet restore

# Публикуем
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Копируем всё, что получилось
COPY --from=build /app/publish .

# ЯВНО копируем wwwroot ещё раз
COPY --from=build /src/wwwroot ./wwwroot

# Проверка в логах
RUN echo "=== Проверка wwwroot ===" && ls -la ./wwwroot

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]