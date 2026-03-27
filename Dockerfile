# Этап 1: сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["MovieRecommendationApp.csproj", "."]
RUN dotnet restore

# Копируем всё остальное
COPY . .

# Публикуем приложение
RUN dotnet publish -c Release -o /app/publish

# Этап 2: запуск
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=build /app/publish .

# Убеждаемся, что wwwroot скопирован
RUN ls -la

# Открываем порт
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]