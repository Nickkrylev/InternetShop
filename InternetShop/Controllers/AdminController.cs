using InternetShop.DataBase;
using InternetShop.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InternetShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private DbRepository dbRepostiroy;
        public AdminController()
        {
            dbRepostiroy = new DbRepository();
        }
        [HttpDelete]
        public IActionResult DeleteProduct([FromBody]string product)
        {
            dbRepostiroy.RemoveProduct(product).Wait();
            return Ok(product);
        }
        [HttpPut]
        public IActionResult EditProduct(Product product)
        {
            dbRepostiroy.UpdateProduct(product);
            return Ok();
        }
    }
}
