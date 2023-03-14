using System.ComponentModel.DataAnnotations;

namespace TrackingAnimal.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string firstName { get; set; } 
        public string lastName { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
        public List<Animal> Animals { get; set; } = new ();
    }
}
