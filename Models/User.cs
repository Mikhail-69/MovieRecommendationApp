using System.ComponentModel.DataAnnotations;

namespace MovieRecommendationApp.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;
    }
}