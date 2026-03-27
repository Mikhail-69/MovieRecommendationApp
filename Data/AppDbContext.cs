using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MovieRecommendationApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Уникальный индекс для логина
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Составной индекс для рейтинга (чтобы один пользователь не мог оценить фильм дважды)
            modelBuilder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.FilmId })
                .IsUnique();

            // Составной индекс для истории
            modelBuilder.Entity<History>()
                .HasIndex(h => new { h.UserId, h.FilmId })
                .IsUnique();
        }
    }
}