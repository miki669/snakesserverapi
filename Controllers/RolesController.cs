using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.DataBase.Data;

namespace SnakeServerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "owner", AuthenticationSchemes = "Token")]
    public class RolesController : ControllerBase
    {
        private readonly SnakeDB _dbContext;

        private readonly ILogger<RolesController> _logger;
        public RolesController(ILogger<RolesController> logger, SnakeDB dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }


        [HttpGet("AddRole")]
        public async Task<ActionResult> AddRole([BindRequired]ulong Role, string Name)
        {
            var dbRole = _dbContext.Roles.Any(p => p.RoleId == Role);
            if (dbRole)
            {
                return Problem("Role already exists");
            }

            _dbContext.Roles.Add(new SnakeRole {
                Name = Name,
                RoleId = Role
            });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("DeleteRole")]
        public async Task<ActionResult> DeleteRole([BindRequired] ulong Role)
        {
            var dbRole = _dbContext.Roles.FirstOrDefault(p => p.RoleId == Role);
            if (dbRole == null)
            {
                return Problem("Role doesnt exists");
            }
            _dbContext.Roles.Remove(dbRole);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("GetUsers")]
        public async Task<ActionResult> Getusers([BindRequired] ulong Role)
        {
            var dbRole = _dbContext.Roles.FirstOrDefault(p => p.RoleId == Role);
            if (dbRole == null)
            {
                return Problem("Role doesnt exists");
            }
            await _dbContext.Entry(dbRole).Collection(p => p.Users).LoadAsync();
            return Ok();

            
        }












        //[HttpPost(Name = "AddRole/{Role}")]
        //public List<SnakeRoles> PutUpdateRole(string Role)
        //{
        //    using SnakeDB db = new();
        //    try
        //    {
        //    var a = db.Roles.FromSqlRaw($"INSERT INTO `roles`(`Role`) VALUES ('{Role}')");

        //        List<SnakeRoles> users = db.Roles.ToList();
        //        return a;

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: [{ex}]");
        //        return new List<SnakeRoles>();
        //    }

        //}


    }




}
