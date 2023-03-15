using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public ActionResult<AnimalTypeDTO> getTypeAnimal(int typeId)
        {
            if (typeId <= 0)
            {
                return BadRequest();
            }
            var typeAnimal = _context.AnimalTypes.AsNoTracking().FirstOrDefault(type => type.Id == typeId);
            if (typeAnimal == null)
            {
                return NotFound();
            }
            var model = new AnimalTypeDTO()
            {
                Id = typeAnimal.Id,
                Type = typeAnimal.Type
            };
            return Ok(model);
        }
        [Authorize]
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

                var model = new AnimalType()
                {
                    Type=animalTypeDTO.Type,
                };
                _context.AnimalTypes.Add(model);
                _context.SaveChanges();
                var olderId = _context.AnimalTypes.OrderByDescending(type => type.Id).FirstOrDefault() != null ? _context.AnimalTypes.OrderByDescending(type => type.Id).FirstOrDefault().Id : 1;
                animalTypeDTO.Id = olderId;
                return CreatedAtRoute("getTypeAnimal", new { typeId = animalTypeDTO.Id }, animalTypeDTO);
            }
            else
            {
                return Conflict();
            }
        }
        [Authorize]
        [HttpPut("{typeId}")]
        public ActionResult<AnimalTypeDTO> updateAccount(int typeId, [FromBody] AnimalTypeDTO animalTypeDTO)
        {
            if (String.IsNullOrWhiteSpace(animalTypeDTO.Type) || !ModelState.IsValid || typeId <=0)
            {
                return BadRequest();
            }
            var animalType = _context.AnimalTypes.FirstOrDefault(type => type.Id == typeId);
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
                var olderId = _context.AnimalTypes.OrderByDescending(t => t.Id).FirstOrDefault()!=null ? _context.AnimalTypes.OrderByDescending(t => t.Id).FirstOrDefault().Id + 1 : 1;
                animalTypeDTO.Id = olderId;
                animalType.Type = animalTypeDTO.Type;
                _context.AnimalTypes.Update(animalType);
                _context.SaveChanges();
                return Ok(animalTypeDTO);
            }
        }
        [Authorize]
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
