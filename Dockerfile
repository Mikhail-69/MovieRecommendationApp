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

# Копируем опубликованное приложение
COPY --from=build /app/publish .

# Проверяем, что wwwroot скопирован
RUN ls -la && ls -la wwwroot || echo "wwwroot not found"

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]