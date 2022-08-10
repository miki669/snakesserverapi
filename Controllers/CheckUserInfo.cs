using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SnakeServerAPI.Assets;
using SnakeServerAPI.DataBase;
using SnakeServerAPI.Encrypt;

namespace SnakeServerAPI.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class CheckUserInfo : ControllerBase
    {
        const string HYZ = "<RSAKeyValue><Modulus>pMoL+iM/Ldkes65piSyBnHWTUhqFvVnoVpEsxRa7O12RWJmxl6z+XU3EoYYO2WldYD+f7HXmtJfdeM6V2Tvvl47W7hjPfXUB64ImNLezX/MiMEf5f9AxMJpg7xVfrJtff5EGtMEsgBHwEjAJ/DxUAgMs1B4L4LcOoBfNSSg8ezi/C+RH56QmxpO6aCGcTPDi792oN81rt9/YuDgPT+0w/B8g6OJ8D9b1krbQ4NSc9eoUhRxxN5E0GIuVeadSM+tta6O2s5/Q1s//xcfyOY1X39ktOdJ9I4Spz03y0bFmzCmeXMOir57nm7wYLR3xF9jXzC6e9H+CUkaa1rKN7g98sQ==</Modulus><Exponent>AQAB</Exponent><P>zGx4VQD+J7hcrdMsCThjXS7wY34va4+SIostcri9Jdy1fSZe4Md13ol7I8hlrl9+euPWvffcA9syAVfuKmMkyKDgLJzQkm+gYg0OYazl300/9pfwb6U59xGK2LJ+q1h/IUwHN9mumDKXHT2WpAC7dY+VydQ5MCpBzhqEpxSaFMc=</P><Q>zl2f+MWM1vx/TAaHeCE27977qvdYURLk15HyBsaAn1jZgOh8uGvVFHLqlJBEq6qSbo8CmpazNjgT1VCXBcTAYfacIO4PhSpgDKIBvgS72mRLnOPGH7A+dr/MbYfQn9RV3NystjfgA2Cutepctw0lZ8yk8fa52/drl9qsqZEJ+sc=</Q><DP>G7xr5zynP0Robr1EMEwST0ZbH2Szkmh8b2tq0zH1l/mGNmDKZr0pZPRKXOSHx0z1oD7hmAzVMUDAXCZODjo9d2s8f1EXi4bRv/v9g4st9UpJ729WFA/a+YbLy/ML9LuhJCdoo09JvSTAFBuTINVomd7NeADvcXWKUCiQ7wqAjWk=</DP><DQ>xATEjj2/oPooRGOzTBVopIJO1T+rIR9sRaN4nJzy0elIeJzc0ySCOrFJRyKeR912yBOJaqOYyPiNRkMNoMoZ3zOra9AU5+2vXHCKbR77/N7lv7nPmIwFWTCoEqY+MYM0p9zpJRB/9VVhvqRFuw5+qFEHoFo2gs1K4uwws8R0EUM=</DQ><InverseQ>yIlX570aZl6C64Yep4vGe5E5M5lgPnVOfWgDtZrF2FlDDD2cOfm1D7HFyMsI+/xibD0tZu62wD5XYahmD7nrEXHzUJmOGPuwxB7dfxmYb6elVwIQyl8ZxLS9oDLmdMk063X9KV+WrqH8QuM3zLqOTE2xOZUb/ENdOeNn62+QjCU=</InverseQ><D>iqoF2A9Ou5rG7fWmkqojym9Zby4oOcRs8GMq5BGTrYm3o3F48iO8yvCEtFJlLmcuDq6bONOhNXfRAjX7/BcmLkcg7qtr9aq+2djjo1qtEuE5dJ0sAnRC4B4X0TcQEeOQQXzcwr8GXiI5/GP2Ew0Et8jKMRezcP04zlgZAyVbd2dYGBG3QaC0iewwmOlnvjmu17iAZOxUPtTG9IWEk1z5eNWSrho7TdB24NwOo43EUEemqbBSEO074LHmtn5dXfbqxzD+mS5zhgs3kIFQI9WNfnMIGuPPgKHk3XhzlNMWYUVJpWVN2+/kuv4iaA2EwZCsaQY1saGjVpOk3tjso4LaDQ==</D></RSAKeyValue>";

        private readonly SnakeDB _dbContext;
        public CheckUserInfo(SnakeDB dbContext)
        {
            _dbContext = dbContext;
        }





        //public string Base64Encode(string plainText)
        //{
        //    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        //    return System.Convert.ToBase64String(plainTextBytes);
        //}

        [Authorize(AuthenticationSchemes = "Token")]
        [HttpGet(Name = "CheckUserRole")]
        public async Task<ActionResult> PutUpdateRole(        
        string?   City,
        string?   IP,
        string?   Hwid,
        string?   Gpu_Name,
        string?   Pc_Name,
        string?   Disck_Info,
        string?   Procces_Id
        )
        {

            var user = _dbContext.Users.First(p => p.Id == User.GetUserId());
            await _dbContext.Entry(user).Collection(p => p.Roles).LoadAsync();
            var roles = user.Roles;
            //var data = _dbContext.MainAppByte.Where(p => p.Data.LongLength >= 0);


            var data = _dbContext.MainAppByte.FirstOrDefault().Data;



            if (user.Roles.Select(p => p.RoleId).Any())
            {

                //StreamContent streamContent=  new(data);
                
                return Ok(data);
                //return Ok();
            }

            
            return Problem("Empty content");
            //await new LoginAttempt().Login_Attempt(DiscordID, remoteIpAddress);


            

            //if (remoteIpAddress.Discord_ID == DiscordID)
            //{
            //    if (_dbContext.Users.Select(p => p.Discord_Id == DiscordID).Any())
            //    {

            //        //var a = await MainSoftAsync();

            //        //string base64String = Convert.ToBase64String(a, 0, a.Length);

            //        ////System.IO.File.
            //        //using (var sw = System.IO.File.AppendText("XYZ.txt"))
            //        //{
            //        //    sw.WriteLine(base64String);
            //        //}
            //        //var v = Base64Encode();
            //        //await Response.Body.WriteAsync(a, 0, a.Length);
            //        //await Response.Body.WriteAsync(base64String);
            //        return Ok("OK");
            //    }
            //    return Problem("User doesnt exists");
            //}
            //else return Problem("Invalid id");



        }

    }

}

