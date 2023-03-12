using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using TrackingAnimal.Data;
using TrackingAnimal.Models;

namespace TrackingAnimal.Controllers
{
    [Route("/accounts")]
    public class AccountController :Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{accountId}")]
        public ActionResult<Account> getAccount(int accountId)
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
                return Ok(account);
            }
            
        }
        [Route("search")]
        [HttpGet]
        public ActionResult<List<Account>> searchAccounts(
            [FromQuery(Name = "firstName")] string firstName,
            [FromQuery(Name = "lastName")] string lastName,
            [FromQuery(Name = "email")] string email,
            [FromQuery(Name = "from")] int from =0,
            [FromQuery(Name = "size")] int size =10
            )
        {
            var accounts = _context.Accounts.ToList().Where(account =>
            {
                Console.WriteLine(account);
                if (account.firstName == null || account.lastName == null || account.email == null)
                {
                    return false;
                }
                if (account.firstName.ToLower().Contains(firstName.ToLower()) && account.lastName.ToLower().Contains(lastName.ToLower()) && account.email.ToLower().Contains(email.ToLower()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }).Skip(from).Take(size);
            Console.WriteLine(accounts);
            return Ok(accounts);
            
        }
        [HttpPut("{accountId}")]
        public ActionResult<Account> updateAccount(int accountId, [FromBody] Account accountDTO)
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
            var account = _context.Accounts.FirstOrDefault(account => account.Id == accountId && account.Id ==accountDTO.Id);
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
                return Ok(account);
            }
        }
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
