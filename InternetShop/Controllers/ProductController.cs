using InternetShop.DataBase;
using InternetShop.DataBase.Models;
using InternetShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InternetShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private DbRepository dbRepository;
        public ProductController()
        {
            dbRepository = new DbRepository();
        }
        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await dbRepository.GetProducts());
        }
        [Route("buy")]
        [HttpPost]
        public IActionResult BuyProduct(BuyModel buyModel)
        {
            if (dbRepository.GetBalance(buyModel.UserName) < dbRepository.GetPrice(buyModel.ProductName) ||
                buyModel.Amount > dbRepository.GetProductAmoun(buyModel.ProductName)
                || dbRepository.GetProductAmoun(buyModel.ProductName) == 0)
                return BadRequest();
            dbRepository.BuyProduct(buyModel).Wait();
            return Ok();
        }
        [Route("user")]
        [HttpGet]
        public async Task<IActionResult> GetUserProducts([FromQuery]string name)
        {
            return Ok(await dbRepository.GetUserProducts(name));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteProduct([FromBody] string product)
        {
            dbRepository.RemoveProduct(product).Wait();
            return Ok();
        }
    }
}
