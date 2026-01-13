using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController(IProductRepository itemRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await itemRepository.ListAllProducts());
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ProductDto body)
        {
            await itemRepository.CreateProduct(body);
            return Ok();
        }
    }
}
