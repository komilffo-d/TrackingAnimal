using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;
using TrackingAnimal.Types;

namespace TrackingAnimal.Controllers
{
    [Route("/animals/{animalId:long}/locations")]
    [ApiController]
    public class LocationVisitedAnimalController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LocationVisitedAnimalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
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
        [HttpPost("{pointId:long}")]
        public ActionResult createLocationVisitedPointByAnimal(long animalId, long pointId)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                var locationPoint = _context.Locations.FirstOrDefault(l => l.Id == pointId);
                if (locationPoint != null)
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
                    return CreatedAtRoute(nameof(getLocationVisitedPointsByAnimal), new() { }, sendModel);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpDelete("{visitedPointId}")]
        public ActionResult deleteLocationVisitedPointsByAnimal(long animalId, long visitedPointId)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(a => a.visitedLocations).Load();
                var visitedlocationPoint = animal.visitedLocations.FirstOrDefault(l => l.Id == visitedPointId);
                if (visitedlocationPoint != null)
                {
                    animal.visitedLocations.Remove(visitedlocationPoint);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpPut]
        public ActionResult changeLocationVisitedPointsByAnimal(long animalId, [FromBody] ChangeVisitedLocation data)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(u => u.visitedLocations).Load();
                var visitedLocationPoint = animal.visitedLocations.FirstOrDefault(l => l.Id == data.visitedLocationPointId);
                var newLocationPoint = _context.Locations.FirstOrDefault(l => l.Id == data.locationPointId);
                if (visitedLocationPoint != null && newLocationPoint != null)
                {
                    ///Удаление старого типа и сохранение
                    animal.visitedLocations.Remove(visitedLocationPoint);
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
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
