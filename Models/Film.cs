using System.ComponentModel.DataAnnotations;

namespace MovieRecommendationApp.Models
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }

        public float Rating { get; set; }

        public int Duration { get; set; }

        public string Poster { get; set; }

        public string Genre { get; set; }

        public string Actors { get; set; }
    }
}