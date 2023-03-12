using System.Globalization;

namespace TrackingAnimal.Models
{
    public class Animal
    {
        public long Id { get; set; }
        
        public AnimalType[] animalTypes { get; set; }

        public float weight { get; set; }
        public float length { get; set; }
        public float height { get; set; }
        public string gender { get; set; }
        public string lifeStatus { get; set; }
        public int chipperId { get; set; }
 
        public long chippingLocationId { get; set; }
        public long[] visitedLocations { get; set; }
        public DateTime chippingDateTime { get; set; } = DateTime.Now;
        public DateTime deathDateTime { get; set; } = DateTime.Now;
    }
}
