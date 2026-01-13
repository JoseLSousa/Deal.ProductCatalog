using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController(IItemRepository itemRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await itemRepository.ListAllItems());
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemDto body)
        {
            await itemRepository.CreateItem(body);
            return Ok();
        }
    }
}
