FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем всё и восстанавливаем зависимости
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Копируем опубликованное приложение (включая wwwroot)
COPY --from=build /app/publish .

# Убеждаемся, что wwwroot скопирован
RUN test -d wwwroot && echo "wwwroot exists" || echo "wwwroot missing"

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "MovieRecommendationApp.dll"]