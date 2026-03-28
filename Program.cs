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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Получаем строку подключения из переменной окружения
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    // Локальная разработка — SQLite
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=movieapp.db"));
}
else
{
    // На Render — PostgreSQL
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<FilmRepo>();
builder.Services.AddScoped<RatingRepo>();
builder.Services.AddScoped<HistoryRepo>();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<QuestService>();
builder.Services.AddSingleton<TMDBService>();

var app = builder.Build();

// Автоматическое создание базы данных и таблиц
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); // Создаёт базу и таблицы, если их нет
}

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
    }
}