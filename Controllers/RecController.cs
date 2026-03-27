using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendationApp.Models;
using MovieRecommendationApp.Repositories;
using MovieRecommendationApp.Services;

namespace MovieRecommendationApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecController : ControllerBase
    {
        private readonly FilmRepo _filmRepo;
        private readonly RatingRepo _ratingRepo;
        private readonly HistoryRepo _historyRepo;
        private readonly QuestService _questService;

        public RecController(FilmRepo filmRepo, RatingRepo ratingRepo, HistoryRepo historyRepo, QuestService questService)
        {
            _filmRepo = filmRepo;
            _ratingRepo = ratingRepo;
            _historyRepo = historyRepo;
            _questService = questService;
        }

        // POST: api/rec/by-quest
        [HttpPost("by-quest")]
        public IActionResult GetByQuest([FromBody] QuestionAnswers answers, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (answers == null)
            {
                return BadRequest("Анкета не заполнена");
            }

            Console.WriteLine($"=== Получена анкета ===");
            Console.WriteLine($"Жанры: {string.Join(", ", answers.Genres)}");
            Console.WriteLine($"Актёры: {string.Join(", ", answers.Actors)}");
            Console.WriteLine($"Год: {answers.MinYear}-{answers.MaxYear}");
            Console.WriteLine($"Рейтинг: {answers.MinRating}");
            Console.WriteLine($"Длительность: до {answers.MaxDuration} мин");
            Console.WriteLine($"Страница: {page}, на странице: {pageSize}");

            // Получаем все фильмы из репозитория
            var allFilms = _filmRepo.GetAllFilms();
            Console.WriteLine($"Всего фильмов в базе: {allFilms.Count}");

            // Фильтруем
            var filteredFilms = allFilms.AsEnumerable();

            // Фильтр по жанрам
            if (answers.Genres != null && answers.Genres.Count > 0)
            {
                filteredFilms = filteredFilms.Where(f =>
                    f.Genre != null &&
                    answers.Genres.Any(g => f.Genre.ToLower().Contains(g.ToLower()))
                );
                Console.WriteLine($"После фильтра по жанрам: {filteredFilms.Count()}");
            }

            // Фильтр по актёрам
            if (answers.Actors != null && answers.Actors.Count > 0)
            {
                filteredFilms = filteredFilms.Where(f =>
                    f.Actors != null &&
                    answers.Actors.Any(a => f.Actors.ToLower().Contains(a.ToLower()))
                );
                Console.WriteLine($"После фильтра по актёрам: {filteredFilms.Count()}");
            }

            // Фильтр по году
            filteredFilms = filteredFilms.Where(f =>
                f.Year >= answers.MinYear && f.Year <= answers.MaxYear
            );
            Console.WriteLine($"После фильтра по году: {filteredFilms.Count()}");

            // Фильтр по рейтингу
            filteredFilms = filteredFilms.Where(f =>
                f.Rating >= answers.MinRating
            );
            Console.WriteLine($"После фильтра по рейтингу: {filteredFilms.Count()}");

            // Фильтр по длительности
            filteredFilms = filteredFilms.Where(f =>
                f.Duration <= answers.MaxDuration
            );
            Console.WriteLine($"После фильтра по длительности: {filteredFilms.Count()}");

            // Сортируем по рейтингу
            var sortedFilms = filteredFilms.OrderByDescending(f => f.Rating).ToList();

            int totalCount = sortedFilms.Count;
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Берём нужную страницу
            var result = sortedFilms
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine($"Всего найдено: {totalCount}, страниц: {totalPages}, показано: {result.Count}");

            // Возвращаем вместе с информацией о пагинации
            return Ok(new
            {
                items = result,
                totalCount = totalCount,
                totalPages = totalPages,
                currentPage = page,
                pageSize = pageSize
            });
        }

        // GET: api/rec/personalized?userId=1
        [HttpGet("personalized")]
        public IActionResult GetPersonalizedRecs([FromQuery] int userId)
        {
            var userRatings = _ratingRepo.GetUserRatings(userId);

            if (userRatings.Count == 0)
            {
                return Ok(new List<Film>());
            }

            var likedGenres = new List<string>();
            foreach (var rating in userRatings.Where(r => r.Score >= 7))
            {
                var film = _filmRepo.GetFilmById(rating.FilmId);
                if (film != null && !string.IsNullOrEmpty(film.Genre))
                {
                    likedGenres.Add(film.Genre);
                }
            }

            var watchedFilms = _historyRepo.GetWatchedFilmIds(userId);
            var recommendations = new List<Film>();

            foreach (var genre in likedGenres.Distinct())
            {
                var films = _filmRepo.SearchFilms(genre: genre);
                films = films.Where(f => !watchedFilms.Contains(f.Id)).ToList();
                recommendations.AddRange(films);
            }

            recommendations = recommendations
                .GroupBy(f => f.Id)
                .Select(g => g.First())
                .OrderByDescending(f => f.Rating)
                .Take(10)
                .ToList();

            return Ok(recommendations);
        }

        // POST: api/rec/rate
        [HttpPost("rate")]
        public IActionResult RateFilm([FromBody] RateRequest request)
        {
            _ratingRepo.AddOrUpdateRating(request.UserId, request.FilmId, request.Score);

            if (request.Score >= 5)
            {
                _historyRepo.AddToHistory(request.UserId, request.FilmId);
            }

            return Ok(new { message = "Оценка сохранена" });
        }

        // GET: api/rec/history?userId=1
        [HttpGet("history")]
        public IActionResult GetHistory([FromQuery] int userId)
        {
            var history = _historyRepo.GetUserHistory(userId);
            var films = new List<Film>();

            foreach (var item in history)
            {
                var film = _filmRepo.GetFilmById(item.FilmId);
                if (film != null)
                {
                    films.Add(film);
                }
            }

            return Ok(films);
        }
    }

    public class RateRequest
    {
        public int UserId { get; set; }
        public int FilmId { get; set; }
        public int Score { get; set; }
    }
}