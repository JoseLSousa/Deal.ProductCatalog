using Application.DTOs;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController(ICategoryRepository categoryRepository) : ControllerBase
    {
        // GET: Leitura para todos
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var categories = await categoryRepository.ListAllCategories(includeDeleted);
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        [HttpGet("deleted")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetDeleted()
        {
            var categories = await categoryRepository.GetDeletedCategories();
            return Ok(categories);
        }

        // POST/PUT: Admin e Editor
        [HttpPost]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task<IActionResult> Create([FromBody] RequestCategoryDto categoryDto)
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

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RequestCategoryDto categoryDto)
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

        // DELETE: Apenas Admin
        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanDelete)]
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

        [HttpPost("{id}/restore")]
        [Authorize(Policy = Policies.CanDelete)]
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

        [HttpPost("{categoryId}/products/{productId}")]
        [Authorize(Policy = Policies.CanWrite)]
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

        [HttpDelete("{categoryId}/products/{productId}")]
        [Authorize(Policy = Policies.CanDelete)]
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