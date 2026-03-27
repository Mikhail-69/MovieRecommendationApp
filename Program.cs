using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Data;
using MovieRecommendationApp.Repositories;
using MovieRecommendationApp.Services;
using Microsoft.OpenApi.Models;  

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args); 

        // Добавляем контроллеры
        builder.Services.AddControllers();

        // Настройка базы данных SQLite
        //builder.Services.AddDbContext<AppDbContext>(options =>
        //  options.UseSqlite("Data Source=movieapp.db"));

        // Стало для PostgreSQL
        var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                               ?? "Host=localhost;Database=movieapp;Username=postgres;Password=postgres";
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));


        // Регистрируем репозитории
        builder.Services.AddScoped<UserRepo>();
        builder.Services.AddScoped<FilmRepo>();
        builder.Services.AddScoped<RatingRepo>();
        builder.Services.AddScoped<HistoryRepo>();

        // Регистрируем сервисы
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<QuestService>();
        builder.Services.AddSingleton<TMDBService>(); // Singleton, т.к. не хранит состояние

        // Добавляем Swagger для тестирования API
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Создаем базу данных, если её нет
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated(); // Создает БД и таблицы
        }

        // Настройка Swagger
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}