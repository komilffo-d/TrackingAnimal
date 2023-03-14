using Microsoft.AspNetCore.Mvc;
using System;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;

namespace TrackingAnimal.Controllers
{
    [Route("/animals/types")]
    [ApiController]
    public class AnimalTypeController :Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{typeId}", Name = nameof(getTypeAnimal))]
        public ActionResult<AnimalType> getTypeAnimal(int typeId)
        {
            if (typeId <= 0)
            {
                return BadRequest();
            }
            var typeAnimal = _context.AnimalTypes.FirstOrDefault(type => type.Id == typeId);
            if (typeAnimal == null)
            {
                return NotFound();
            }
            return Ok(typeAnimal);
        }
        [HttpPost]
        public ActionResult<AnimalTypeDTO> addLocationPoint([FromBody] AnimalTypeDTO animalTypeDTO)
        {
            if (String.IsNullOrWhiteSpace(animalTypeDTO.Type))
            {
                return BadRequest();
            }
            var animalType = _context.AnimalTypes.FirstOrDefault(type => type.Type == animalTypeDTO.Type);
            if (animalType == null)
            {
                var lostId = _context.AnimalTypes.OrderByDescending(type => type.Id).FirstOrDefault() != null ? _context.AnimalTypes.OrderByDescending(type => type.Id).FirstOrDefault().Id + 1 : 1;
                animalTypeDTO.Id = lostId;
                var model = new AnimalType()
                {
                    Type=animalTypeDTO.Type,
                };
                _context.AnimalTypes.Add(model);
                _context.SaveChanges();
                return CreatedAtRoute("getTypeAnimal", new { typeId = animalTypeDTO.Id }, animalTypeDTO);
            }
            else
            {
                return Conflict();
            }
        }
        [HttpPut("{typeId}")]
        public ActionResult<AnimalType> updateAccount(int typeId, [FromBody] AnimalType animalTypeDTO)
        {
            if (String.IsNullOrWhiteSpace(animalTypeDTO.Type) || !ModelState.IsValid || typeId <=0)
            {
                return BadRequest();
            }
            var animalType = _context.AnimalTypes.FirstOrDefault(type => type.Id == animalTypeDTO.Id && type.Id == typeId);
            if (animalType == null)
            {
                return NotFound();
            }
            if (animalType.Type == animalTypeDTO.Type)
            {
                return Conflict();
            }
            else
            {
                animalType.Type = animalTypeDTO.Type;
                _context.AnimalTypes.Update(animalType);
                _context.SaveChanges();
                return Ok(animalTypeDTO);
            }
        }
        [HttpDelete("{typeId}")]
        public ActionResult deleteAccount(int typeId)
        {

            if (typeId <= 0)
            {
                return BadRequest();
            }
            var animalType = _context.AnimalTypes.FirstOrDefault(type => type.Id == typeId);
            if (animalType == null)
            {
                return NotFound();
            }
            else
            {
                _context.AnimalTypes.Remove(animalType);
                _context.SaveChanges();
                return Ok();
            }
        }
    }
}
