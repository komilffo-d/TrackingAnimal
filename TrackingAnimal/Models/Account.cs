using System.ComponentModel.DataAnnotations;

namespace TrackingAnimal.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; } 
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
