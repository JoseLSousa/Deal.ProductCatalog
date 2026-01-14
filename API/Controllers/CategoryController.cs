using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryRepository categoryRepository) : ControllerBase
    {
        // GET: api/category
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var categories = await categoryRepository.ListAllCategories(includeDeleted);
            return Ok(categories);
        }

        // GET: api/category/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        // GET: api/category/deleted
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var categories = await categoryRepository.GetDeletedCategories();
            return Ok(categories);
        }

        // POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryDto categoryDto)
        {
            try
            {
                await categoryRepository.CreateCategory(categoryDto);
                return Ok(new { message = "Categoria criada com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar categoria.", detail = ex.Message });
            }
        }

        // PUT: api/category/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto categoryDto)
        {
            try
            {
                await categoryRepository.UpdateCategory(id, categoryDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await categoryRepository.DeleteCategory(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/category/{id}/restore
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await categoryRepository.RestoreCategory(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/category/{categoryId}/products/{productId}
        [HttpPost("{categoryId}/products/{productId}")]
        public async Task<IActionResult> AddProduct(Guid categoryId, Guid productId)
        {
            try
            {
                await categoryRepository.AddProductToCategory(categoryId, productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/category/{categoryId}/products/{productId}
        [HttpDelete("{categoryId}/products/{productId}")]
        public async Task<IActionResult> RemoveProduct(Guid categoryId, Guid productId)
        {
            try
            {
                await categoryRepository.RemoveProductFromCategory(categoryId, productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}