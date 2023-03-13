using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;

namespace TrackingAnimal.Controllers
{
    [Route("/registration")]
    [ApiController]
    public class RegistrationController:Controller
    {
        private readonly ApplicationDbContext _context;
        public RegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public ActionResult<Account> registerAccount([FromBody] Account accountDTO)
        {
            if (
                String.IsNullOrWhiteSpace(accountDTO.firstName) ||
                String.IsNullOrWhiteSpace(accountDTO.lastName) ||
                String.IsNullOrWhiteSpace(accountDTO.email) ||
                String.IsNullOrWhiteSpace(accountDTO.password) ||
                !ModelState.IsValid)
            {
                return BadRequest();
            }
            if(accountDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            var account = _context.Accounts.FirstOrDefault(account => account.email ==accountDTO.email) ;
            if (account == null)
            {
                _context.Accounts.Add(accountDTO);
                _context.SaveChanges();
                return CreatedAtAction("getAccount", "Account", new {  id = accountDTO.Id }, accountDTO);
            }
            if (account.email == accountDTO.email)
            {
                return Conflict();
            }
            return NotFound();
        }
    }
}
