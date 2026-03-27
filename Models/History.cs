using System.ComponentModel.DataAnnotations;

namespace MovieRecommendationApp.Models
{
    public class History
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int FilmId { get; set; }

        public DateTime WatchedAt { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } = true;
    }
}