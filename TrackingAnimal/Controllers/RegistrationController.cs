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
        public ActionResult<AccountDTO> registerAccount([FromBody] AccountDTO accountDTO)
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
            var account = _context.Accounts.FirstOrDefault(account => account.email ==accountDTO.email) ;
            if (account == null)
            {
                var model = new Account()
                {
                    firstName = accountDTO.firstName,
                    lastName = accountDTO.lastName,
                    email = accountDTO.email,
                    password = accountDTO.password
                };
                _context.Accounts.Add(model);
                _context.SaveChanges();

                var olderId = _context.Accounts.OrderByDescending(a => a.Id).FirstOrDefault()!=null ? _context.Accounts.OrderByDescending(a => a.Id).FirstOrDefault().Id +1 : 1;
                var sendModel = new AccountDTO()
                {
                    Id = olderId,
                    firstName = accountDTO.firstName,
                    lastName = accountDTO.lastName,
                    email = accountDTO.email
                };
                return CreatedAtAction("getAccount", "Account", new { accountId = sendModel.Id }, sendModel);
            }
            else
            {
                return Conflict();
            }
        }
    }
}
