using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;
using TrackingAnimal.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrackingAnimal.Controllers
{
    [Route("/animals")]
    [ApiController]
    public class LocationVisitedAnimalController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LocationVisitedAnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("{animalId:long}/locations")]
        [HttpGet(Name = nameof(getLocationVisitedPointsByAnimal))]
        public ActionResult getLocationVisitedPointsByAnimal(
            long animalId,
            [FromQuery(Name = "startDateTime")] DateTime? startDateTime = null,
            [FromQuery(Name = "endDateTime")] DateTime? endDateTime = null,
            [FromQuery(Name = "from")] int from = 0,
            [FromQuery(Name = "size")] int size = 10
        )
        {
            if (animalId == null || animalId <= 0 || from < 0 || size <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(a => a.visitedLocations).Load();
                var visitedLocetionsPoints = animal.visitedLocations.Select(l =>
                {
                    return new LocationVisitedAnimalDTO()
                    {
                        Id = l.Id,
                        dateTimeOfVisitLocationPoint = l.dateTimeOfVisitLocationPoint,
                        LocationPointId = l.LocationPointId
                    };
                });

                return Ok(visitedLocetionsPoints);

            }
            else
            {
                return NotFound();
            }
        }
        [Authorize]
        [HttpPost("{animalId:long}/locations/{pointId:long}")]
        public ActionResult createLocationVisitedPointByAnimal(long? animalId, long? pointId)
        {
            if (animalId == null || animalId <= 0 || pointId == null || pointId <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            var locationPoint = _context.Locations.FirstOrDefault(l => l.Id == pointId);
            _context.Entry(locationPoint).Collection(l => l.Animals).Load();
            if (animal.lifeStatus == "DEAD" || locationPoint.Animals.Count() > 0)
            {
                return BadRequest();
            }
            if (animal.visitedLocations.Count() == 0 && pointId == animal.chippingLocationId)
            {
                return BadRequest();
            }
            if (animal != null && locationPoint != null)
            {
                var model = new LocationVisitedAnimal()
                {
                    dateTimeOfVisitLocationPoint = DateTime.Now,
                    LocationPointId = (long)pointId
                };
                _context.Entry(animal).Collection(a => a.visitedLocations).Load();
                animal.visitedLocations.Add(model);
                _context.SaveChanges();


                var sendModel = new LocationVisitedAnimalDTO()
                {
                    Id = model.Id,
                    dateTimeOfVisitLocationPoint = model.dateTimeOfVisitLocationPoint,
                    LocationPointId = model.LocationPointId
                };
                return CreatedAtRoute(nameof(getLocationVisitedPointsByAnimal), new { animalId = model.Id }, sendModel);
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize]
        [HttpDelete("{animalId:long}/locations/{visitedPointId}")]
        public ActionResult deleteLocationVisitedPointsByAnimal(long? animalId, long? visitedPointId)
        {
            if (animalId <= 0 || animalId == null ||
                visitedPointId == null || visitedPointId <=0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            _context.Entry(animal).Collection(a => a.visitedLocations).Load();
            var visitedlocationPointByAnimal = animal.visitedLocations.FirstOrDefault(l => l.Id == visitedPointId);
            var visitedLocationPoint = _context.locationVisitedAnimals.FirstOrDefault(l => l.Id == visitedPointId);
            if (animal != null  && visitedlocationPointByAnimal!=null && visitedLocationPoint != null)
            {
                    animal.visitedLocations.Remove(visitedlocationPointByAnimal);
                    _context.SaveChanges();
                    return Ok();
            }
            else
            {
                return NotFound();
            }
        }
        [Authorize]
        [HttpPut("{animalId:long}/locations")]
        public ActionResult changeLocationVisitedPointsByAnimal(long? animalId, [FromBody] ChangeVisitedLocation data)
        {
            if (animalId <= 0 || animalId == null ||
                data.visitedLocationPointId == null || data.visitedLocationPointId <= 0 ||
                data.locationPointId == null || data.visitedLocationPointId <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            var visitedLocationPoint = _context.locationVisitedAnimals.FirstOrDefault(l => l.Id == data.visitedLocationPointId);
            var newLocationPoint = _context.Locations.FirstOrDefault(l => l.Id == data.locationPointId);
            _context.Entry(animal).Collection(u => u.visitedLocations).Load();
            var visitedLocationPointByAnimal = animal.visitedLocations.FirstOrDefault(l => l.Id == data.visitedLocationPointId);

            if (newLocationPoint.Id == visitedLocationPointByAnimal.LocationPointId)
            {
                return BadRequest();
            }
            if (animal != null && visitedLocationPoint != null)
            {

                if (visitedLocationPointByAnimal != null && newLocationPoint != null)
                {
                    ///Удаление старого типа и сохранение
                    animal.visitedLocations.Remove(visitedLocationPointByAnimal);
                    _context.SaveChanges();

                    ///Добавление нового типа и сохранение
                    var model = new LocationVisitedAnimal()
                    {
                        dateTimeOfVisitLocationPoint = DateTime.Now,
                        LocationPointId = (long)data.visitedLocationPointId
                    };
                    animal.visitedLocations.Add(model);
                    _context.SaveChanges();


                    var sendModel = new LocationVisitedAnimalDTO()
                    {
                        Id = model.Id,
                        dateTimeOfVisitLocationPoint = model.dateTimeOfVisitLocationPoint,
                        LocationPointId = model.LocationPointId
                    };
                    return Ok(sendModel);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
