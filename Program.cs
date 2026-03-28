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
var connectionString = Environment.GetEnvironmentVariable("postgresql://movieapp_db_9bbo_user:SizUZGwgnkvFKnpgAIbLpw6Sa5OwfhTI@dpg-d73e6nma2pns73fivp9g-a.frankfurt-postgres.render.com/movieapp_db_9bbo");

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