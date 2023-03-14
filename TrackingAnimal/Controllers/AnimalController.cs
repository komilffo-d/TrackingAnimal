using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using TrackingAnimal.Data;
using TrackingAnimal.Types;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;
using TrackingAnimal.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TrackingAnimal.Controllers
{
    [ApiController]
    [Route("/animals")]
    public class AnimalController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{animalId:long}", Name = nameof(getAnimal))]
        public ActionResult<AnimalDTO> getAnimal(int animalId)
        {
            if (animalId <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal == null)
            {
                return NotFound();
            }
            else
            {

                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                _context.Entry(animal).Collection(u => u.visitedLocations).Load();
                var listTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray();

                var sendModel = new AnimalDTO()
                {
                    Id = animal.Id,
                    animalTypes = listTypes,
                    weight = animal.weight,
                    length = animal.length,
                    height = animal.height,
                    gender = animal.gender,
                    chipperId = (int)animal.chipperId,
                    lifeStatus = "ALIVE",
                    chippingLocationId = (int)animal.chippingLocationId,
                    chippingDateTime = animal.chippingDateTime,
                    deathDateTime = animal.deathDateTime,
                    visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray(),
                };

                return Ok(sendModel);
            }
        }
        [Route("search")]
        [HttpGet]
        public ActionResult<List<AccountDTO>> searchAccounts(
            [FromQuery(Name = "startDateTime")] DateTime? startDateTime = null,
            [FromQuery(Name = "endDateTime")] DateTime? endDateTime = null,
            [FromQuery(Name = "chipperId")] int? chipperId = null,
            [FromQuery(Name = "chippingLocationId")] long? chippingLocationId = null,
            [FromQuery(Name = "lifeStatus")] string lifeStatus = null,
            [FromQuery(Name = "gender")] string gender = null,
            [FromQuery(Name = "from")] int from = 0,
            [FromQuery(Name = "size")] int size = 10
        )
        {
            if (from < 0 || size <= 0)
            {
                return BadRequest();
            }
            var animals = _context.Animals.ToList().Where(animal =>
              {
                  if ((animal.gender == gender || gender == null) &&
                      (animal.lifeStatus == lifeStatus || lifeStatus == null) &&
                      (animal.chippingLocationId == chippingLocationId || chippingLocationId == null) &&
                      (animal.chipperId == chipperId || chipperId == null) &&
                      (animal.chippingDateTime >= startDateTime || startDateTime == null) &&
                      (animal.chippingDateTime <= endDateTime || endDateTime == null))
                  {
                      return true;
                  }
                  else
                      return false;
              }).Skip(from).Take(size).OrderBy(animal => animal.Id);
            var model = animals.ToList().Select(animal =>
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                _context.Entry(animal).Collection(u => u.visitedLocations).Load();
                var listTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray();
                return new AnimalDTO()
                {
                    Id = animal.Id,
                    animalTypes = listTypes,
                    weight = animal.weight,
                    length = animal.length,
                    height = animal.height,
                    gender = animal.gender,
                    chipperId = (int)animal.chipperId,
                    lifeStatus = "ALIVE",
                    chippingLocationId = (int)animal.chippingLocationId,
                    chippingDateTime = animal.chippingDateTime,
                    deathDateTime = animal.deathDateTime,
                    visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray(),
                };
            });


            return Ok(model);

        }
        [HttpPost]
        public ActionResult<AnimalDTO> createAnimal([FromBody] AnimalDTO animalDTO)
        {
            var olderId = _context.Animals.AsNoTracking().OrderByDescending(animal => animal.Id).FirstOrDefault() != null ? _context.Animals.OrderByDescending(animal => animal.Id).FirstOrDefault().Id + 1 : 1;
#pragma warning disable CS8619 
            var model = new Animal()
            {
                animalTypes = animalDTO.animalTypes.Select(TypeId =>
                {
                    var animalDbType = _context.AnimalTypes.FirstOrDefault(animalType => animalType.Id == TypeId);
                    if (animalDbType != null)
                        _context.Entry(animalDbType).Collection(a => a.Animal).Load();
                    return animalDbType;
                }).ToList(),
                weight = animalDTO.weight,
                length = animalDTO.length,
                height = animalDTO.height,
                gender = animalDTO.gender,
                chipperId = animalDTO.chipperId,
                lifeStatus = "ALIVE",
                chippingLocationId = animalDTO.chippingLocationId
            };
#pragma warning restore CS8619 
            _context.Animals.Add(model);
            _context.SaveChanges();
            _context.Entry(model).Collection(m => m.visitedLocations).Load();
            var sendModel = new AnimalDTO()
            {
                Id = olderId,
                animalTypes = animalDTO.animalTypes,
                weight = animalDTO.weight,
                length = animalDTO.length,
                height = animalDTO.height,
                gender = animalDTO.gender,
                chipperId = animalDTO.chipperId,
                lifeStatus = "ALIVE",
                chippingLocationId = animalDTO.chippingLocationId,
                chippingDateTime = model.chippingDateTime,
                deathDateTime = model.deathDateTime,
                visitedLocations = model.visitedLocations.Select(a => a.Id).ToArray(),
            };
            return Ok(sendModel);
        }
        [HttpPut("{animalId}")]
        public ActionResult<AnimalDTO> updateAnimal(int animalId, [FromBody] AnimalDTO animalDTO)
        {
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            animal.weight = animalDTO.weight;
            animal.length = animalDTO.length;
            animal.height = animalDTO.height;
            animal.gender = animalDTO.gender;
            animal.lifeStatus = animalDTO.lifeStatus;
            animal.chipperId = animalDTO.chipperId;
            animal.chippingLocationId = animalDTO.chippingLocationId;
            _context.Animals.Update(animal);
            _context.SaveChanges();
            _context.Entry(animal).Collection(u => u.animalTypes).Load();
            _context.Entry(animal).Collection(u => u.visitedLocations).Load();
            var sendModel = new AnimalDTO()
            {
                Id = animal.Id,
                animalTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray(),
                weight = animal.weight,
                length = animal.length,
                height = animal.height,
                gender = animal.gender,
                chipperId = (int)animal.chipperId,
                lifeStatus = animal.lifeStatus,
                chippingLocationId = (int)animal.chippingLocationId,
                chippingDateTime = animal.chippingDateTime,
                deathDateTime = animal.deathDateTime,
                visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray(),
            };
            return Ok(sendModel);
        }
        [HttpDelete("{animalId}")]
        public ActionResult<AnimalDTO> deleteAnimal(int animalId)
        {
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);

            _context.Animals.Remove(animal);
            _context.SaveChanges();

            return Ok();
        }
        [HttpPost("{animalId:long}/types/{typeId:long}")]
        public ActionResult changeTypeForAnimal(int animalId, int typeId)
        {
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                var animalTypeNew = _context.AnimalTypes.FirstOrDefault(at => at.Id == typeId);
                if (animalTypeNew != null)
                { 
                    animal.animalTypes.Add(animalTypeNew);
                    _context.SaveChanges();
                    _context.Entry(animal).Collection(u => u.visitedLocations).Load();
                    var sendModel = new AnimalDTO()
                    {
                        Id = animal.Id,
                        animalTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray(),
                        weight = animal.weight,
                        length = animal.length,
                        height = animal.height,
                        gender = animal.gender,
                        chipperId = (int)animal.chipperId,
                        lifeStatus = animal.lifeStatus,
                        chippingLocationId = (int)animal.chippingLocationId,
                        chippingDateTime = animal.chippingDateTime,
                        deathDateTime = animal.deathDateTime,
                        visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray(),
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
                return NotFound();
            }
        }
        [HttpPut("{animalId}/types")]
        public ActionResult changeTypeForAnimal(int animalId, [FromBody] ChangeType data)
        {
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                var animalType = animal.animalTypes.FirstOrDefault(at => at.Id == data.oldTypeId);
                if (animalType != null)
                {
                    ///Удаление старого типа и сохранение
                    animal.animalTypes.Remove(animalType);
                    _context.SaveChanges();
                    ///нахождение сущности типа с новым ID
                    var animalTypeNew = _context.AnimalTypes.FirstOrDefault(animalType => animalType.Id == data.newTypeId);

                    ///Добавление нового типа и сохранение
                    animal.animalTypes.Add(animalTypeNew);
                    _context.SaveChanges();


                    _context.Entry(animal).Collection(u => u.visitedLocations).Load();
                    var sendModel = new AnimalDTO()
                    {
                        Id = animal.Id,
                        animalTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray(),
                        weight = animal.weight,
                        length = animal.length,
                        height = animal.height,
                        gender = animal.gender,
                        chipperId = (int)animal.chipperId,
                        lifeStatus = animal.lifeStatus,
                        chippingLocationId = (int)animal.chippingLocationId,
                        chippingDateTime = animal.chippingDateTime,
                        deathDateTime = animal.deathDateTime,
                        visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray(),
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
        [HttpDelete("{animalId:long}/types/{typeId:long}")]
        public ActionResult<AnimalDTO> deleteTypeForAnimal(long animalId, long typeId)
        {
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                var animalType = animal.animalTypes.FirstOrDefault(at => at.Id == typeId);
                if (animalType != null)
                {
                    animal.animalTypes.Remove(animalType);
                    _context.SaveChanges();
                    return CreatedAtRoute(nameof(getAnimal), new { animalId = animalId }, new { });
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

