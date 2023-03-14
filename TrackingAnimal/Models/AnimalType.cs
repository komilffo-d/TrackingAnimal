namespace TrackingAnimal.Models
{
    public class AnimalType
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public List<Animal> Animal{ get; set; } = new List<Animal>();
    }
}
