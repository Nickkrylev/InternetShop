using InternetShop.DataBase;
using InternetShop.DataBase.Models;
using InternetShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InternetShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private DbRepository dbRepository;
        public UserController()
        {
            dbRepository = new DbRepository();
        }
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> CreateAccount(User user)
        {
            await dbRepository.AddUser(new UserModel
            {
                Name = user.Name,
                Balance = user.Balance
            });
            return Ok();
        }
        [Route("login")]
        [HttpPost]
        public IActionResult Login([FromBody]string userName)
        {
            LoggedUser user = new LoggedUser()
            {
                Index = 0
            };
            if (dbRepository.IsUserExist(userName))
            {
                user.Balance = dbRepository.GetBalance(userName);
                if (userName.Contains("admin"))
                {
                    user.Index = 2;
                    return Ok(user);
                }
                user.Index = 1;
                return Ok(user);
            }

            return Ok(user);
        }
        [Route("logout")]
        [HttpPost]
        public IActionResult Logout(string userName)
        {
            return Ok();
        }
        [Route("balance")]
        [HttpPut]
        public IActionResult EditBalance([FromBody]User user)
        {
            dbRepository.EditBalance(user).Wait();
            return Ok();
        }
    }
}
