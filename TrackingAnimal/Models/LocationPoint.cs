using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TrackingAnimal.Models
{
    public class LocationPoint
    {
        [Key]
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<Animal> ?Animals { get; set; } = new();
        public List<LocationVisitedAnimal> ? LocationVisited { get; set; } = new();
    }
}
