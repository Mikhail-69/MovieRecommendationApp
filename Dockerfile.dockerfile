# Этап 1: сборка
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY ["MovieRecommendationApp.csproj", "."]
RUN dotnet restore

# Копируем всё остальное и собираем
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Этап 2: запуск
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Открываем порт для Render
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]