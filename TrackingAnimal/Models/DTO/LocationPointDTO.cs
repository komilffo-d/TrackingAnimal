using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TrackingAnimal.Models.DTO
{
    public class LocationPointDTO
    {
        [Key]
        public long Id { get; set; }
        [AllowNull]
        public double Latitude { get; set; }
        [AllowNull]
        public double Longitude { get; set; }
    }
}
