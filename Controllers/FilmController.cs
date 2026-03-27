using Microsoft.AspNetCore.Mvc;
using MovieRecommendationApp.Repositories;
using MovieRecommendationApp.Services;

namespace MovieRecommendationApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmController : ControllerBase
    {
        private readonly FilmRepo _filmRepo;
        private readonly TMDBService _tmdbService;

        public FilmController(FilmRepo filmRepo, TMDBService tmdbService)
        {
            _filmRepo = filmRepo;
            _tmdbService = tmdbService;
        }

        // GET: api/film/search?query=Матрица&genre=боевик&year=1999
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query = null, [FromQuery] string genre = null, [FromQuery] int? year = null)
        {
            var films = _filmRepo.SearchFilms(genre, year, null, query);

            if (films.Count == 0 && !string.IsNullOrEmpty(query))
            {
                // Отладочный вывод
                Console.WriteLine($"Поиск в TMDb для запроса: {query}");

                var tmdbFilms = await _tmdbService.SearchFilms(query);

                Console.WriteLine($"Найдено фильмов в TMDb: {tmdbFilms.Count}");

                foreach (var film in tmdbFilms)
                {
                    _filmRepo.AddFilm(film);
                }

                return Ok(tmdbFilms);
            }

            return Ok(films);
        }

        // GET: api/film/details/5
        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var film = _filmRepo.GetFilmById(id);

            if (film == null)
            {
                // Если фильма нет в своей базе, пробуем получить из TMDb
                var tmdbFilm = await _tmdbService.GetFilmDetails(id);
                if (tmdbFilm != null)
                {
                    _filmRepo.AddFilm(tmdbFilm);
                    return Ok(tmdbFilm);
                }

                return NotFound(new { message = "Фильм не найден" });
            }

            return Ok(film);
        }

        // GET: api/film/popular
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular()
        {
            var films = _filmRepo.GetAllFilms();
            return Ok(films.OrderByDescending(f => f.Rating).Take(20));
        }
    }
}