using System.ComponentModel.DataAnnotations;

namespace MovieRecommendationApp.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FilmId { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }  // оценка от 1 до 10

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}