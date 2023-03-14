using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;

namespace TrackingAnimal.Controllers
{

    [Route("/locations")]
    [ApiController]
    public class LocationPointController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LocationPointController(ApplicationDbContext context) {
            _context = context;
         }

        [HttpGet("{pointId}",Name = nameof(getLocationPoint))]
        public ActionResult<LocationPointDTO> getLocationPoint(int pointId)
        {
            if(pointId <= 0)
            {
                return BadRequest();    
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Id == pointId);
            if (locationPoint == null)
            {
                return NotFound();
            }
            var model = new LocationPointDTO()
            {
                Id = locationPoint.Id,
                Latitude = locationPoint.Latitude,
                Longitude = locationPoint.Longitude
            };
            return Ok(model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult<LocationPointDTO> addLocationPoint([FromBody] LocationPointDTO locationPointDTO)
        {
            if (locationPointDTO.Latitude ==null || 
                locationPointDTO.Latitude < -90 || 
                locationPointDTO.Latitude > 90 || 
                locationPointDTO.Longitude == null || 
                locationPointDTO.Longitude < -180 || 
                locationPointDTO.Longitude > 180 )
            {
                return BadRequest();
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Longitude == locationPointDTO.Longitude && point.Latitude == locationPointDTO.Latitude);
            if (locationPoint == null)
            {

                var model = new LocationPoint()
                {
                    Latitude = locationPointDTO.Latitude,
                    Longitude = locationPointDTO.Longitude
                };
                _context.Locations.Add(model);
                _context.SaveChanges();
                var olderId = _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault() != null ? _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault().Id: 1;
                var sendModel = new LocationPointDTO()
                {
                    Id = olderId,
                    Latitude = locationPointDTO.Latitude,
                    Longitude = locationPointDTO.Longitude
                };
                return CreatedAtRoute("getLocationPoint", new { pointId = sendModel.Id}, sendModel);
            }   
            else
            {
                return Conflict();
            }
        }
        [Authorize]
        [HttpPut("{pointId}")]
        public ActionResult<LocationPointDTO> updateAccount(int pointId, [FromBody] LocationPointDTO locationPointDTO)
        {
            if (
                pointId <= 0 || 
                locationPointDTO.Latitude == null ||
                locationPointDTO.Latitude < -90 ||
                locationPointDTO.Latitude > 90 ||
                locationPointDTO.Longitude == null ||
                locationPointDTO.Longitude < -180 ||
                locationPointDTO.Longitude > 180 ||
                !ModelState.IsValid)
            {
                return BadRequest();
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Id == pointId);
            if (locationPoint == null)
            {
                return NotFound();
            }
            if (locationPoint.Longitude == locationPointDTO.Longitude && locationPoint.Latitude == locationPointDTO.Latitude)
            {
                return Conflict();
            }
            else
            {
                locationPoint.Longitude = locationPointDTO.Longitude;
                locationPoint.Latitude = locationPointDTO.Latitude;
                _context.Locations.Update(locationPoint);
                _context.SaveChanges();
                var olderId = _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault() != null ? _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault().Id : 1;
                var sendModel = new LocationPointDTO()
                {
                    Id = olderId,
                    Latitude = locationPoint.Latitude,
                    Longitude = locationPoint.Longitude

                };
                return Ok(sendModel);    
            }
        }
        [Authorize]
        [HttpDelete("{pointId}")]
        public ActionResult deleteAccount(int pointId)
        {

            if (pointId <= 0)
            {
                return BadRequest();
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Id == pointId);
            if (locationPoint == null)
            {
                return NotFound();
            }
            else
            {
                _context.Locations.Remove(locationPoint);
                _context.SaveChanges();
                return Ok();
            }
        }
    }
}
