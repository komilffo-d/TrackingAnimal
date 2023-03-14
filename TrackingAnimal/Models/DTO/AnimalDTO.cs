using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TrackingAnimal.Models.DTO
{
    public class AnimalDTO
    {
        public long Id { get; set; }

        public long[] animalTypes { get; set; } 

        public float weight { get; set; }
        public float length { get; set; }
        public float height { get; set; }
        public string gender { get; set; }
        public int chipperId { get; set; }

        public long chippingLocationId { get; set; }
    }
}
