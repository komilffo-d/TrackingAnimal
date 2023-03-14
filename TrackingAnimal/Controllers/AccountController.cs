    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using TrackingAnimal.Data;
using TrackingAnimal.Models;
using TrackingAnimal.Models.DTO;

namespace TrackingAnimal.Controllers
{
    
    [Route("/accounts")]
    [ApiController]
    public class AccountController :Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        [HttpGet("{accountId}", Name = "getAccount")]
        public ActionResult<AccountDTO> getAccount(int accountId)
        {
            if (accountId == null  || accountId <= 0)
            {
                return BadRequest();
            }
            var account = _context.Accounts.FirstOrDefault(account => account.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }
            else
            {
                var model = new AccountDTO()
                {
                    Id = account.Id,
                    firstName = account.firstName,
                    lastName = account.lastName,
                    email = account.email,
                };
                return Ok(model);
            }
            
        }
        [Route("search")]
        [HttpGet]
        public ActionResult<List<AccountDTO>> searchAccounts(
            [FromQuery(Name = "firstName")] string ? firstName,
            [FromQuery(Name = "lastName")] string ? lastName,
            [FromQuery(Name = "email")] string ? email ,
            [FromQuery(Name = "from")] int from =0,
            [FromQuery(Name = "size")] int size =10
            )
        {
            if(from < 0 || size <= 0)
            {
                return BadRequest();
            }
            var accounts = _context.Accounts.ToList().Where(account =>
            {

                if ((firstName==null || account.firstName.ToLower().Contains(firstName.ToLower())) && (lastName == null || account.lastName.ToLower().Contains(lastName.ToLower()) ) && (email == null || account.email.ToLower().Contains(email.ToLower()) ))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).Skip(from).Take(size).OrderBy(account => account.Id);
            var model = accounts.ToList().Select(account =>
                 {
                     return new AccountDTO()
                           {
                                    Id = account.Id,
                                    firstName = account.firstName,
                                    lastName = account.lastName,
                                    email = account.email
                            };
                 });
             

            return Ok(model);
            
        }
        [Authorize]
        [HttpPut("{accountId}")]
        public ActionResult<AccountDTO> updateAccount(int accountId, [FromBody] AccountDTO accountDTO)
        {
            if (
                accountId <= 0 || 
                String.IsNullOrWhiteSpace(accountDTO.firstName) || 
                String.IsNullOrWhiteSpace(accountDTO.lastName) ||
                String.IsNullOrWhiteSpace(accountDTO.email)  ||
                String.IsNullOrWhiteSpace(accountDTO.password) || 
                !ModelState.IsValid)
            {
                return BadRequest();
            }
            var account = _context.Accounts.FirstOrDefault(account => account.Id == accountId);
            if (account == null)
            {
                return NotFound();
            }
            if (account.email == accountDTO.email)
            {
                return Conflict();
            }
            else
            {
                account.email = accountDTO.email;
                account.firstName = accountDTO.firstName;
                account.lastName = accountDTO.lastName;
                account.password=accountDTO.password;
                _context.Accounts.Update(account);
                _context.SaveChanges();
                var model = new AccountDTO()
                {
                    Id = account.Id,
                    firstName = account.firstName,
                    lastName = account.lastName,
                    email = account.email
                };
                return Ok(model);
            }
        }
        [Authorize]
        [HttpDelete("{accountId}")]
        public ActionResult deleteAccount(int accountId)
        {
            
            if(accountId <= 0)
            {
                return BadRequest();
            }
            var account = _context.Accounts.FirstOrDefault(account => account.Id == accountId );
            if (account == null)
            {
                return NotFound();
            }
            else
            {
                _context.Accounts.Remove(account);
                _context.SaveChanges();
                return Ok();
            }
        }

    }
}
