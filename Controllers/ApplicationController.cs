using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.DataBase.Data;

namespace SnakeServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "owner", AuthenticationSchemes = "Token")]
    public class ApplicationController : ControllerBase
    {
        private readonly SnakeDB _dbContext;

        private readonly ILogger<RolesController> _logger;
        public ApplicationController(ILogger<RolesController> logger, SnakeDB dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        [HttpGet("AddApp")]
        public async Task<ActionResult> AddApp(string content)
        {
            var dbRole = _dbContext.MainAppByte.Any(p => p.Data == content);
            if (dbRole)
            {
                return Problem("Application already exists");
            }

            _dbContext.MainAppByte.Add(new BytesApplication
            {
                Data = content,
                DateTime = DateTime.Now,
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
