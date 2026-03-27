using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Data;
using MovieRecommendationApp.Models;


namespace MovieRecommendationApp.Repositories
{
    public class HistoryRepo
    {
        private readonly AppDbContext _context;

        public HistoryRepo(AppDbContext context)
        {
            _context = context;
        }

        // Добавить фильм в историю просмотров
        public void AddToHistory(int userId, int filmId)
        {
            if (!_context.Histories.Any(h => h.UserId == userId && h.FilmId == filmId))
            {
                _context.Histories.Add(new History
                {
                    UserId = userId,
                    FilmId = filmId,
                    WatchedAt = DateTime.Now
                });
                _context.SaveChanges();
            }
        }

        // Получить историю просмотров пользователя
        public List<History> GetUserHistory(int userId)
        {
            return _context.Histories
                .Where(h => h.UserId == userId)
                .ToList();
        }

        // Получить ID фильмов, которые пользователь уже смотрел
        public List<int> GetWatchedFilmIds(int userId)
        {
            return _context.Histories
                .Where(h => h.UserId == userId)
                .Select(h => h.FilmId)
                .ToList();
        }

        // Проверить, смотрел ли пользователь фильм
        public bool HasUserWatchedFilm(int userId, int filmId)
        {
            return _context.Histories
                .Any(h => h.UserId == userId && h.FilmId == filmId);
        }
    }
}