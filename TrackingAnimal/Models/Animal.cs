using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TrackingAnimal.Models
{
    public class Animal
    {

        public long Id { get; set; }

        public List<AnimalType> animalTypes { get; set; } = new List<AnimalType>();

        public float weight { get; set; }
        public float length { get; set; }
        public float height { get; set; }
        public string gender { get; set; }
        public string lifeStatus { get; set; } = "ALIVE";

        public int? chipperId { get; set; }
        public Account? chipper { get; set; }

        public long? chippingLocationId { get; set; }
        public LocationPoint? chippingLocation { get; set; }

        public List<LocationVisitedAnimal> visitedLocations { get; set; } = new List<LocationVisitedAnimal>();

        public DateTime chippingDateTime { get; set; } = DateTime.Now;
        public DateTime ? deathDateTime { get; set; } = null;
    }
}
