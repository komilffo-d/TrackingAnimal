namespace TrackingAnimal.Models.DTO
{
    public class LocationVisitedAnimalDTO
    {
        public long Id { get; set; }
        public DateTime dateTimeOfVisitLocationPoint { get; set; }
        public long? LocationPointId { get; set; }
    }
}
