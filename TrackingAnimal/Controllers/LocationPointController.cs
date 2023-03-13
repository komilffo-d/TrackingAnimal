using Microsoft.AspNetCore.Mvc;
using TrackingAnimal.Data;
using TrackingAnimal.Models;

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
        public ActionResult<LocationPoint> getLocationPoint(int pointId)
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
            return Ok(locationPoint);
        }
        [HttpPost]
        public ActionResult<LocationPoint> addLocationPoint([FromBody] LocationPoint locationPointDTO)
        {
            if (locationPointDTO.Lalitude ==null || 
                locationPointDTO.Lalitude < -90 || 
                locationPointDTO.Lalitude > 90 || 
                locationPointDTO.Longitude == null || 
                locationPointDTO.Longitude < -180 || 
                locationPointDTO.Longitude > 180 )
            {
                return BadRequest();
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Longitude == locationPointDTO.Longitude && point.Lalitude == locationPointDTO.Lalitude);
            if (locationPoint == null)
            {
                var lostId = _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault()!=null ? _context.Locations.OrderByDescending(point => point.Id).FirstOrDefault().Id + 1 : 1;
                locationPointDTO.Id = lostId;
                var model = new LocationPoint()
                {
                    Lalitude = locationPointDTO.Lalitude,
                    Longitude = locationPointDTO.Longitude
                };
                _context.Locations.Add(model);
                _context.SaveChanges();
                return CreatedAtRoute("getLocationPoint", new { pointId =locationPointDTO.Id},locationPointDTO);
            }   
            else
            {
                return Conflict();
            }
        }
        [HttpPut("{pointId}")]
        public ActionResult<Account> updateAccount(int pointId, [FromBody] LocationPoint locationPointDTO)
        {
            if (
                pointId <= 0 || 
                locationPointDTO.Lalitude == null ||
                locationPointDTO.Lalitude < -90 ||
                locationPointDTO.Lalitude > 90 ||
                locationPointDTO.Longitude == null ||
                locationPointDTO.Longitude < -180 ||
                locationPointDTO.Longitude > 180 ||
                !ModelState.IsValid)
            {
                return BadRequest();
            }
            var locationPoint = _context.Locations.FirstOrDefault(point => point.Id == pointId && point.Id == locationPointDTO.Id);
            if (locationPoint == null)
            {
                return NotFound();
            }
            if (locationPoint.Longitude == locationPointDTO.Longitude && locationPoint.Lalitude == locationPointDTO.Lalitude)
            {
                return Conflict();
            }
            else
            {
                locationPoint.Longitude = locationPointDTO.Longitude;
                locationPoint.Lalitude = locationPointDTO.Lalitude;
                _context.Locations.Update(locationPoint);
                _context.SaveChanges();
                return Ok(locationPointDTO);    
            }
        }
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
