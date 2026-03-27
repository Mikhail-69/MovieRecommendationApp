using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Data;
using MovieRecommendationApp.Models;
using System;

namespace MovieRecommendationApp.Repositories
{
    public class RatingRepo
    {
        private readonly AppDbContext _context;

        public RatingRepo(AppDbContext context)
        {
            _context = context;
        }

        // Добавить или обновить оценку
        public bool AddOrUpdateRating(int userId, int filmId, int score)
        {
            var existing = _context.Ratings
                .FirstOrDefault(r => r.UserId == userId && r.FilmId == filmId);

            if (existing != null)
            {
                existing.Score = score;
                existing.CreatedAt = DateTime.Now;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    UserId = userId,
                    FilmId = filmId,
                    Score = score
                });
            }

            _context.SaveChanges();
            return true;
        }

        // Получить все оценки пользователя
        public List<Rating> GetUserRatings(int userId)
        {
            return _context.Ratings
                .Where(r => r.UserId == userId)
                .ToList();
        }

        // Получить среднюю оценку фильма
        public float GetFilmAverageRating(int filmId)
        {
            var ratings = _context.Ratings.Where(r => r.FilmId == filmId);
            if (!ratings.Any()) return 0;
            return (float)ratings.Average(r => r.Score);
        }

        // Получить оценку пользователя для фильма
        public int GetUserRatingForFilm(int userId, int filmId)
        {
            var rating = _context.Ratings
                .FirstOrDefault(r => r.UserId == userId && r.FilmId == filmId);
            return rating?.Score ?? 0;
        }
    }
}