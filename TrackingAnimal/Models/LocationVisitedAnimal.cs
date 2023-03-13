using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TrackingAnimal.Models
{
    public class LocationVisitedAnimal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime dateTimeOfVisitLocationPoint { get; set; }   
        public LocationPoint LocationPoint { get; set; }

    }
}
