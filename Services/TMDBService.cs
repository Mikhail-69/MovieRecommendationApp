using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MovieRecommendationApp.Models;

namespace MovieRecommendationApp.Services
{
    public class TMDBService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";

        public TMDBService()
        {
            _httpClient = new HttpClient();
            _apiKey = "5dab74868303e4a31265fbd9436a3410";
        }

        // Поиск фильмов по названию
        public async Task<List<Film>> SearchFilms(string query)
        {
            Console.WriteLine($"=== Поиск в TMDb: {query} ===");
            try
            {
                // Ищем на английском, чтобы найти фильм
                string url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}";
                Console.WriteLine($"URL: {url}");

                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"HTTP статус: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка! Код: {response.StatusCode}");
                    return new List<Film>();
                }

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Получен JSON, длина: {json.Length} символов");

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (!root.TryGetProperty("results", out var resultsElement))
                {
                    Console.WriteLine("Поле 'results' не найдено в JSON");
                    return new List<Film>();
                }

                var films = new List<Film>();
                int count = 0;

                foreach (var item in resultsElement.EnumerateArray())
                {
                    count++;

                    int id = item.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;

                    // Сначала загружаем русскую версию
                    Film film = await GetFilmDetails(id);

                    if (film != null)
                    {
                        films.Add(film);
                    }
                    else
                    {
                        // Если русская версия не загрузилась, используем данные из поиска
                        string title = item.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? "Название неизвестно" : "Название неизвестно";

                        int year = 0;
                        if (item.TryGetProperty("release_date", out var dateProp))
                        {
                            string releaseDate = dateProp.GetString();
                            if (!string.IsNullOrEmpty(releaseDate))
                            {
                                if (DateTime.TryParse(releaseDate, out var date))
                                    year = date.Year;
                                else if (releaseDate.Length >= 4 && int.TryParse(releaseDate.Substring(0, 4), out var y))
                                    year = y;
                            }
                        }

                        string overview = item.TryGetProperty("overview", out var overviewProp) ? overviewProp.GetString() ?? "" : "";
                        float rating = item.TryGetProperty("vote_average", out var ratingProp) ? (float)ratingProp.GetDouble() : 0;

                        string posterPath = item.TryGetProperty("poster_path", out var posterProp) ? posterProp.GetString() ?? "" : "";
                        string poster = !string.IsNullOrEmpty(posterPath) ? $"https://image.tmdb.org/t/p/w500{posterPath}" : "";

                        string genreStr = "";
                        if (item.TryGetProperty("genre_ids", out var genresProp) && genresProp.ValueKind == JsonValueKind.Array)
                        {
                            var genreIds = new List<int>();
                            foreach (var g in genresProp.EnumerateArray())
                            {
                                genreIds.Add(g.GetInt32());
                            }
                            if (genreIds.Count > 0)
                                genreStr = string.Join(", ", genreIds.Take(3));
                        }

                        films.Add(new Film
                        {
                            Id = id,
                            Title = title,
                            Year = year,
                            Description = overview,
                            Rating = rating,
                            Poster = poster,
                            Genre = genreStr,
                            Duration = 0,
                            Actors = ""
                        });
                    }
                }

                Console.WriteLine($"Создано объектов Film: {films.Count}");
                return films;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"Стек: {ex.StackTrace}");
                return new List<Film>();
            }
        }

        // Получить детальную информацию о фильме по ID на русском языке
        public async Task<Film> GetFilmDetails(int id)
        {
            try
            {
                // Запрос на русском языке с актёрами
                string url = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language=ru-RU&append_to_response=credits";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Год из даты релиза
                int year = 0;
                if (root.TryGetProperty("release_date", out var dateProp))
                {
                    string releaseDate = dateProp.GetString();
                    if (!string.IsNullOrEmpty(releaseDate))
                    {
                        if (DateTime.TryParse(releaseDate, out var date))
                            year = date.Year;
                        else if (releaseDate.Length >= 4 && int.TryParse(releaseDate.Substring(0, 4), out var y))
                            year = y;
                    }
                }

                // Название на русском
                string title = root.TryGetProperty("title", out var titleProp) ? titleProp.GetString() ?? "Название неизвестно" : "Название неизвестно";

                // Описание на русском
                string overview = root.TryGetProperty("overview", out var overviewProp) ? overviewProp.GetString() ?? "" : "";

                // Рейтинг
                float rating = root.TryGetProperty("vote_average", out var ratingProp) ? (float)ratingProp.GetDouble() : 0;

                // Длительность
                int runtime = root.TryGetProperty("runtime", out var runtimeProp) ? runtimeProp.GetInt32() : 0;

                // Постер
                string posterPath = root.TryGetProperty("poster_path", out var posterProp) ? posterProp.GetString() ?? "" : "";
                string poster = !string.IsNullOrEmpty(posterPath) ? $"https://image.tmdb.org/t/p/w500{posterPath}" : "";

                // Жанры на русском
                string genreStr = "";
                if (root.TryGetProperty("genres", out var genresProp) && genresProp.ValueKind == JsonValueKind.Array)
                {
                    var genreNames = new List<string>();
                    foreach (var g in genresProp.EnumerateArray())
                    {
                        if (g.TryGetProperty("name", out var nameProp))
                        {
                            string name = nameProp.GetString();
                            if (!string.IsNullOrEmpty(name))
                                genreNames.Add(name);
                        }
                    }
                    if (genreNames.Count > 0)
                        genreStr = string.Join(", ", genreNames.Take(3));
                }

                // Актёры на русском
                string actorsStr = "";
                if (root.TryGetProperty("credits", out var creditsProp) &&
                    creditsProp.TryGetProperty("cast", out var castProp) &&
                    castProp.ValueKind == JsonValueKind.Array)
                {
                    var actorNames = new List<string>();
                    int actorCount = 0;
                    foreach (var actor in castProp.EnumerateArray())
                    {
                        if (actorCount >= 8) break;

                        if (actor.TryGetProperty("name", out var actorNameProp))
                        {
                            string actorName = actorNameProp.GetString();
                            if (!string.IsNullOrEmpty(actorName))
                                actorNames.Add(actorName);
                        }
                        actorCount++;
                    }
                    if (actorNames.Count > 0)
                        actorsStr = string.Join(", ", actorNames);
                }

                return new Film
                {
                    Id = id,
                    Title = title,
                    Year = year,
                    Description = overview,
                    Rating = rating,
                    Duration = runtime,
                    Poster = poster,
                    Genre = genreStr,
                    Actors = actorsStr
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в GetFilmDetails: {ex.Message}");
                return null;
            }
        }
    }
}