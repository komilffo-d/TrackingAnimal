using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TrackingAnimal.Models.DTO
{
    public class AnimalDTO
    {
        public long Id { get; set; }

        public long[] ? animalTypes { get; set; } 

        public float weight { get; set; }
        public float length { get; set; }
        public float height { get; set; }
        public string gender { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? lifeStatus { get; set; } = "ALIVE";
        public int chipperId { get; set; }

        public long chippingLocationId { get; set; }

        public long[] visitedLocations { get; set; } =Array.Empty<long>();

        public DateTime chippingDateTime { get; set; } = DateTime.Now;
        public DateTime? deathDateTime { get; set; } = null;
    }
}
