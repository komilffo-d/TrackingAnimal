using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TrackingAnimal.Controllers
{
    [ApiController]
    [Route("/animals")]
    public class AnimalController: Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{animalId:long}")]
        public ActionResult<Animal> getAnimal(int animalId)
        {
            if(animalId <= 0)
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
                var listTypes = animal.animalTypes.Select(animalType => animalType.Id).ToArray();

                var model = new AnimalDTO()
                {
                    Id = animal.Id,
                    animalTypes= listTypes
                };
                
                return Ok(model);
            }
        }
        [HttpPost]
        public ActionResult<Animal> createAnimal([FromBody] AnimalDTO animalDTO)
        {
            var animalOldId= _context.Animals.OrderByDescending(animal => animal.Id).FirstOrDefault() != null ? _context.Animals.OrderByDescending(animal => animal.Id).FirstOrDefault().Id + 1 : 1;
#pragma warning disable CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
            var model = new Animal()
            {
                animalTypes = animalDTO.animalTypes.Select(TypeId =>
                {
                    var animalDbType= _context.AnimalTypes.FirstOrDefault(animalType => animalType.Id == TypeId);
                    _context.Entry(animalDbType).Collection(a => a.Animal).Load();
                    return animalDbType;
                }).ToList(),
                weight=animalDTO.weight,
                length=animalDTO.length,
                height=animalDTO.height,
                gender=animalDTO.gender,
                chipperId=animalDTO.chipperId,
                lifeStatus="ALIVE",
                chippingLocationId =animalDTO.chippingLocationId
            };
#pragma warning restore CS8619 // Допустимость значения NULL для ссылочных типов в значении не соответствует целевому типу.
            _context.Animals.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }
    }
}
