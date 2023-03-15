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
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Microsoft.VisualBasic;

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
        public ActionResult<AnimalDTO> getAnimal(int? animalId)
        {
            if (animalId <= 0 || animalId == null)
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
                var listTypes = animal.animalTypes.Select(animalType => animalType.Id).Where(a => a != null).ToArray();

                var sendModel = new AnimalDTO()
                {
                    Id = animal.Id,
                    animalTypes = listTypes,
                    weight = animal.weight,
                    length = animal.length,
                    height = animal.height,
                    gender = animal.gender,
                    chipperId = animal.chipperId,
                    lifeStatus = "ALIVE",
                    chippingLocationId = animal.chippingLocationId,
                    chippingDateTime = animal.chippingDateTime,
                    deathDateTime = animal.deathDateTime,
                    visitedLocations = animal.visitedLocations.Select(a => a.Id).ToArray()
                };

                return Ok(sendModel);
            }
        }
        [Route("search")]
        [HttpGet]
        public ActionResult<List<AccountDTO>> searchAccounts(
            [FromQuery(Name = "startDateTime")] DateTime startDateTime,
            [FromQuery(Name = "endDateTime")] DateTime endDateTime,
            [FromQuery(Name = "chipperId")] int? chipperId = null,
            [FromQuery(Name = "chippingLocationId")] long? chippingLocationId = null,
            [FromQuery(Name = "lifeStatus")] string lifeStatus = null,
            [FromQuery(Name = "gender")] string gender = null,
            [FromQuery(Name = "from")] int from = 0,
            [FromQuery(Name = "size")] int size = 10
        )
        {
            if (from < 0 || size <= 0 || (lifeStatus != "DEAD" && lifeStatus != "ALIVE") ||
                (gender != "FEMALE" && gender != "MALE" && gender != "OTHER") ||
                chipperId <= 0 || chipperId == null ||
                chippingLocationId <= 0 || chippingLocationId == null)
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
        [Authorize]
        [HttpPost]
        public ActionResult<AnimalDTO> createAnimal([FromBody] AnimalDTO animalDTO)
        {
            if (animalDTO.animalTypes == null ||
                animalDTO.animalTypes.Length <= 0 ||
                animalDTO.animalTypes.Any(t => t == null || t <= 0) ||
                animalDTO.weight <= 0 || animalDTO.weight == null ||
                animalDTO.length <= 0 || animalDTO.length == null ||
                animalDTO.height <= 0 || animalDTO.height == null ||
                animalDTO.gender == null || (animalDTO.gender != "FEMALE" && animalDTO.gender != "MALE" && animalDTO.gender != "OTHER")
                || animalDTO.chipperId <= 0 || animalDTO.chipperId == null ||
                animalDTO.chippingLocationId <= 0 || animalDTO.chippingLocationId == null
                )
            {
                return BadRequest();
            }
            if (animalDTO.animalTypes.Any(typeDTO =>
            {
                return _context.AnimalTypes.FirstOrDefault(type => type.Id == typeDTO) == null;
            }) || _context.Accounts.FirstOrDefault(account => account.Id == animalDTO.chipperId) == null ||
            _context.Locations.FirstOrDefault(location => location.Id == animalDTO.chippingLocationId) == null
            )
            {
                return NotFound(animalDTO);
            }

#pragma warning disable CS8619 
            var model = new Animal()
            {
                animalTypes = animalDTO.animalTypes.Select(TypeId =>
                {
                    var animalDbType = _context.AnimalTypes.FirstOrDefault(animalType => animalType.Id == TypeId);
                    if (animalDbType != null)
                        _context.Entry(animalDbType).Collection(a => a.Animal).Load();
                    return animalDbType;
                }).Where(t => t != null).ToList(),
                weight = animalDTO.weight,
                length = animalDTO.length,
                height = animalDTO.height,
                gender = animalDTO.gender,
                chipperId = _context.Accounts.FirstOrDefault(a => a.Id == animalDTO.chipperId) != null ? animalDTO.chipperId : null,
                lifeStatus = "ALIVE",
                chippingLocationId = _context.Locations.FirstOrDefault(a => a.Id == animalDTO.chippingLocationId) != null ? animalDTO.chippingLocationId : null
            };
#pragma warning restore CS8619 
            _context.Animals.Add(model);
            _context.SaveChanges();
            _context.Entry(model).Collection(m => m.visitedLocations).Load();
            var olderId = _context.Animals.OrderByDescending(animal => animal.Id).FirstOrDefault() != null ? _context.Animals.OrderByDescending(animal => animal.Id).FirstOrDefault().Id : 1;
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
            return CreatedAtRoute(nameof(getAnimal), new { animalId = olderId }, sendModel);
        }
        [Authorize]
        [HttpPut("{animalId}")]
        public ActionResult<AnimalDTO> updateAnimal(int? animalId, [FromBody] AnimalDTO animalDTO)
        {
            if (
                animalId == null || animalId <= 0 ||
                animalDTO.weight <= 0 || animalDTO.weight == null ||
                animalDTO.length <= 0 || animalDTO.length == null ||
                animalDTO.height <= 0 || animalDTO.height == null ||
                animalDTO.gender == null || (animalDTO.gender != "FEMALE" && animalDTO.gender != "MALE" && animalDTO.gender != "OTHER")
                || animalDTO.chipperId <= 0 || animalDTO.chipperId == null ||
                animalDTO.chippingLocationId <= 0 || animalDTO.chippingLocationId == null
                || (animalDTO.lifeStatus != "ALIVE" && animalDTO.lifeStatus != "DEAD")
            )
            {
                return BadRequest("неверные входные данные!");
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            var account = _context.Accounts.FirstOrDefault(a => a.Id == animalDTO.chipperId);
            var chippingLocation = _context.Locations.FirstOrDefault(l => l.Id == animalDTO.chippingLocationId);
            if (animal == null || account == null || chippingLocation == null)
            {
                return NotFound();
            }
            _context.Entry(animal).Collection(a => a.visitedLocations).Load();
            var firstPointAnimal = animal.visitedLocations.OrderBy(l => l.Id).Select(l => l.Id).FirstOrDefault();
            if (animalDTO.chippingLocationId == firstPointAnimal)
            {
                return BadRequest("Совпадают точки локации!");
            }
            animal.weight = animalDTO.weight;
            animal.length = animalDTO.length;
            animal.height = animalDTO.height;
            animal.gender = animalDTO.gender;
            animal.lifeStatus = animalDTO.lifeStatus;
            animal.chipperId = animalDTO.chipperId;
            animal.chippingLocationId = animalDTO.chippingLocationId;
            if (animal.lifeStatus == "DEAD")
                animal.deathDateTime = DateTime.Now;
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
        [Authorize]
        [HttpDelete("{animalId}")]
        public ActionResult<AnimalDTO> deleteAnimal(int? animalId)
        {
            if (animalId == null || animalId <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal == null)
            {
                return NotFound();
            }
            _context.Entry(animal).Collection(a => a.visitedLocations).Load();
            var animalsVisited = animal.visitedLocations.Where(a => a != null);
            if (animalsVisited.Count() > 0)
            {
                return BadRequest();
            }
            _context.Animals.Remove(animal);
            _context.SaveChanges();

            return Ok();
        }
        [Authorize]
        [HttpPost("{animalId:long}/types/{typeId:long}")]
        public ActionResult changeTypeForAnimal(int animalId, int typeId)
        {
            if(animalId <=0 || animalId==null || typeId <=0 || typeId == null)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            if (animal != null)
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                var animalTypeNew = _context.AnimalTypes.FirstOrDefault(at => at.Id == typeId);
                if (animalTypeNew != null)
                {
                    var animalTypeNewByAnimal = animal.animalTypes.FirstOrDefault(t => t.Id == animalTypeNew.Id);
                    if (animalTypeNewByAnimal != null)
                        return Conflict();
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
                    return CreatedAtRoute(nameof(getAnimal),new {animalId= animal.Id },sendModel);
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
        [Authorize]
        [HttpPut("{animalId}/types")]
        public ActionResult changeTypeForAnimal(int? animalId, [FromBody] ChangeType data)
        {
            if (animalId == null || animalId <= 0 || data.oldTypeId <= 0 || data.oldTypeId == null || data.newTypeId == null || data.newTypeId <= 0)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            var animalTypeNew = _context.AnimalTypes.FirstOrDefault(animalType => animalType.Id == data.newTypeId);
            var animalTypeOld = _context.AnimalTypes.FirstOrDefault(at => at.Id == data.oldTypeId);
            if (animal != null && animalTypeNew != null && animalTypeOld != null)
            {
                _context.Entry(animal).Collection(u => u.animalTypes).Load();

                var animalTypeOldByAnimal = animal.animalTypes.FirstOrDefault(t => t.Id == data.oldTypeId);
                var animalTypeNewByAnimal = animal.animalTypes.FirstOrDefault(t => t.Id == data.newTypeId);

                if (animalTypeOldByAnimal != null)
                {
                    if (animalTypeNewByAnimal != null)
                    {
                        return Conflict();
                    }
                    animal.animalTypes.Remove(animalTypeOldByAnimal);
                    _context.SaveChanges();
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
        [Authorize]
        [HttpDelete("{animalId:long}/types/{typeId:long}")]
        public ActionResult<AnimalDTO> deleteTypeForAnimal(long animalId, long typeId)
        {
            if (animalId <= 0 || animalId == null || typeId <= 0 || typeId == null)
            {
                return BadRequest();
            }
            var animal = _context.Animals.FirstOrDefault(animal => animal.Id == animalId);
            var animalType = _context.AnimalTypes.FirstOrDefault(t => t.Id == typeId);
            if (animal != null && animalType!=null)
            {
                
                _context.Entry(animal).Collection(u => u.animalTypes).Load();
                if (animal.animalTypes.Count() ==1 && animal.animalTypes.First().Id == typeId)
                {
                    return BadRequest();
                }
                    var animalTypeByAnimal = animal.animalTypes.FirstOrDefault(at => at.Id == typeId);
                if (animalTypeByAnimal != null)
                {
                    animal.animalTypes.Remove(animalTypeByAnimal);
                    _context.SaveChanges();
                    return Ok(animal);
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

