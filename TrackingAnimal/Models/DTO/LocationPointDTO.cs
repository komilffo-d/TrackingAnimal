using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TrackingAnimal.Models.DTO
{
    public class LocationPointDTO
    {
        [Key]
        public long Id { get; set; }
        [AllowNull]
        public double Lalitude { get; set; }
        [AllowNull]
        public double Longitude { get; set; }
    }
}
