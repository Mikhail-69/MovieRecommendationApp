using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MovieRecommendationApp.Data;
using MovieRecommendationApp.Models;

namespace MovieRecommendationApp.Repositories
{
    public class FilmRepo
    {
        private readonly AppDbContext _context;

        public FilmRepo(AppDbContext context)
        {
            _context = context;
        }

        // Поиск фильмов по разным критериям
        public List<Film> SearchFilms(string genre = null, int? year = null, int? minRating = null, string query = null)
        {
            // Получаем все фильмы из базы
            var films = _context.Films.ToList();

            // Фильтрация в памяти
            var result = films.AsEnumerable();

            // Поиск по названию (частичное совпадение)
            if (!string.IsNullOrEmpty(query))
            {
                result = result.Where(f => f.Title != null &&
                    f.Title.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            // Поиск по жанру (поддержка нескольких жанров через запятую)
            if (!string.IsNullOrEmpty(genre))
            {
                var genresList = genre.Split(',')
                    .Select(g => g.Trim().ToLower())
                    .Where(g => !string.IsNullOrEmpty(g))
                    .ToList();

                if (genresList.Count > 0)
                {
                    result = result.Where(f => f.Genre != null &&
                        genresList.Any(g => f.Genre.ToLower().Contains(g)));
                }
            }

            // Поиск по году
            if (year.HasValue)
            {
                result = result.Where(f => f.Year == year.Value);
            }

            // Поиск по минимальному рейтингу
            if (minRating.HasValue)
            {
                result = result.Where(f => f.Rating >= minRating.Value);
            }

            return result.ToList();
        }

        // Получить фильм по ID
        public Film GetFilmById(int id)
        {
            return _context.Films.Find(id);
        }

        // Добавить фильм (при синхронизации с TMDb)
        public void AddFilm(Film film)
        {
            try
            {
                bool exists = _context.Films.Any(f => f.Title == film.Title && f.Year == film.Year);

                if (!exists)
                {
                    _context.Films.Add(film);
                    _context.SaveChanges();
                    Console.WriteLine($"Фильм '{film.Title}' добавлен в базу");
                }
                else
                {
                    Console.WriteLine($"Фильм '{film.Title}' уже существует");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        // Получить все фильмы
        public List<Film> GetAllFilms()
        {
            return _context.Films.ToList();
        }

        // Получить фильмы по списку ID
        public List<Film> GetFilmsByIds(List<int> ids)
        {
            return _context.Films.Where(f => ids.Contains(f.Id)).ToList();
        }
    }
}