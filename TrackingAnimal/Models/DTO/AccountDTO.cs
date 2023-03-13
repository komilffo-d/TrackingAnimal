using System.ComponentModel.DataAnnotations;

namespace TrackingAnimal.Models.DTO
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
    }
}
