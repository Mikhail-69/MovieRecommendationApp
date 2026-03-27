using MovieRecommendationApp.Models;
using MovieRecommendationApp.Repositories;

namespace MovieRecommendationApp.Services
{
    // Класс для ответов на анкету
    public class QuestionAnswers
    {
        public List<string> Genres { get; set; } = new List<string>();
        public int MinYear { get; set; } = 1900;
        public int MaxYear { get; set; } = DateTime.Now.Year;
        public int MinRating { get; set; } = 0;
        public int MaxDuration { get; set; } = 180;
        public List<string> Actors { get; set; } = new List<string>();
    }

    public class QuestService
    {
        private readonly FilmRepo _filmRepo;

        public QuestService(FilmRepo filmRepo)
        {
            _filmRepo = filmRepo;
        }

        // Собрать ответы анкеты и вернуть подходящие фильмы
        public List<Film> CollectAnswers(QuestionAnswers answers)
        {
            var results = new List<Film>();

            // Поиск по каждому выбранному жанру
            foreach (var genre in answers.Genres)
            {
                var films = _filmRepo.SearchFilms(
                    genre: genre,
                    year: answers.MinYear,
                    minRating: answers.MinRating
                );

                // Фильтрация по длительности
                films = films.Where(f => f.Duration <= answers.MaxDuration).ToList();

                results.AddRange(films);
            }

            // Убираем дубликаты
            return results.GroupBy(f => f.Id).Select(g => g.First()).ToList();
        }

        // Получить список вопросов для анкеты (для фронтенда)
        public List<string> GetQuestions()
        {
            return new List<string>
            {
                "Какие жанры вам нравятся?",
                "Какой год выпуска?",
                "Минимальный рейтинг?",
                "Максимальная длительность?"
            };
        }
    }
}