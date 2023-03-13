using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using TrackingAnimal.Data;

namespace TrackingAnimal.Handlers
{
    public class BasicAuthenticationHandler:AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ApplicationDbContext _context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,ApplicationDbContext context) : base(options, logger, encoder, clock)
        {
            _context = context;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Fail!");
            }
            try
            {
                var headerAutho = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(headerAutho.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");

                var login = credentials[0];
                var password = credentials[1];

                var user = _context.Accounts.Where(user => user.email == login && user.password == password).FirstOrDefault();
                if (user != null)
                {
                    var claims = new[] { new Claim(ClaimTypes.Email, user.email) };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("Invalid data for entire!");
                }
            }
            catch (Exception)
            {

            }
            return AuthenticateResult.Fail("Fail");
        }
    }
}
