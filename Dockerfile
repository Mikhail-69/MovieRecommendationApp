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

# Выводим список файлов в лог (отладка)
RUN echo "=== Содержимое /app ===" && ls -la && echo "=== Содержимое wwwroot ===" && (ls -la wwwroot || echo "wwwroot не найден!")

EXPOSE 8080
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]