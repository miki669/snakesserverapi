using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.DataBase.Data;
using System.Linq;

namespace SnakeServerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly SnakeDB _dbContext;
        private readonly Random rnd;
        private readonly ILogger<RolesController> _logger;
        public AuthController(ILogger<RolesController> logger, SnakeDB dbContext, Random rnd)
        {
            _logger = logger;
            _dbContext = dbContext;
            this.rnd = rnd;
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = "/auth/info" });
        }


        [Authorize]
        [HttpGet("getToken")]
        public async Task<ActionResult> GetToken()
        {
            ulong dc_userId;
            if (!ulong.TryParse(User.Claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value, out dc_userId))
            {
                return Unauthorized();
            }
            var user = this._dbContext.Users.FirstOrDefault(p => p.Discord_Id == dc_userId);
            if (user == null)
            {
                return Unauthorized();
            }
            await _dbContext.Entry(user).Collection(p => p.Tokens).LoadAsync();
            // Check existing token
            UserToken token = user.Tokens.FirstOrDefault(p => p.TokenExpirationTime > DateTime.Now);
            if (token == null)
            {
                token = new UserToken
                {
                    TokenExpirationTime = DateTime.Now.AddDays(1),
                    Token = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 128).Select(s => s[rnd.Next(s.Length)]).ToArray())
                };
                user.Tokens.Add(token);
                await this._dbContext.SaveChangesAsync();
            }
            return Ok(new
            {
                token = token.Token,
                expire_in = (int)(DateTime.Now - token.TokenExpirationTime).TotalSeconds
            });
        }




        [Authorize]
        [HttpGet("info")]
        public IActionResult Info()
        {
            return Ok(new
            {
                Id = User.Claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value,
                Name = User.Claims.FirstOrDefault(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value,
                Discriminator = User.Claims.FirstOrDefault(p => p.Type == "urn:discord:user:discriminator")?.Value
            });
        }
    }
}
