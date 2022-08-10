using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SnakeServerAPI.DataBase;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SnakeServerAPI
{
    public static class UserExtenstion
    {
        public static long GetUserId(this ClaimsPrincipal me)
        {
            string id_s = me.Claims.FirstOrDefault(p => p.Type == "userid")?.Value;
            return long.Parse(id_s);
        }
    }
    public class TokenAuthOptions : AuthenticationSchemeOptions
    {
        public TokenAuthOptions() { }
    }
    public class TokenAuthHandler : AuthenticationHandler<TokenAuthOptions>
    {
        private readonly IServiceProvider serviceProvider;

        public TokenAuthHandler(
            IOptionsMonitor<TokenAuthOptions> options,
            IServiceProvider serviceProvider,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Token") || string.IsNullOrEmpty(Request.Headers["Token"]))
            {
                return AuthenticateResult.Fail("Unauthorized, no token produced");
            }

            try
            {
                return await ValidateToken(Request.Headers["Token"]);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> ValidateToken(string token)
        {
            using var scope = this.serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SnakeDB>();
            var validatedToken = await context.Tokens.FirstOrDefaultAsync(t => t.Token == token && t.TokenExpirationTime > DateTime.Now);
            if (validatedToken == null)
                return AuthenticateResult.Fail("Unauthorized, invalid token");
            await context.Entry(validatedToken).Reference(p => p.SnakeUser).LoadAsync();
            await context.Entry(validatedToken.SnakeUser).Collection(p => p.Roles).LoadAsync();
            string[] roles = validatedToken.SnakeUser.Roles.Select(p => p.Name.ToLower()).ToArray();

            var claims = new List<Claim>
            {
                new Claim("userid", validatedToken.SnakeUser.Id.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new System.Security.Principal.GenericPrincipal(identity, roles);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
