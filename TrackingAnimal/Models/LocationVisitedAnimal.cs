using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TrackingAnimal.Models
{
    public class LocationVisitedAnimal
    {

        public long Id { get; set; }
        public DateTime dateTimeOfVisitLocationPoint { get; set; }   

        public long? LocationPointId { get; set; }
        public LocationPoint ? LocationPoint { get; set; }

        public List<Animal> Animal { get; set; } = new List<Animal>();

    }
}
