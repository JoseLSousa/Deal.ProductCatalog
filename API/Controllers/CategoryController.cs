using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController(ICategoryApplicationService categoryService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await categoryService.GetActiveCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("with-active-products")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetWithActiveProducts()
        {
            try
            {
                var categories = await categoryService.GetCategoriesWithActiveProductsAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar categorias.", detail = ex.Message });
            }
        }

        [HttpGet("{id}", Name = nameof(GetCategoryById))]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        [HttpGet("{id}/with-products")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetWithProducts(Guid id)
        {
            var category = await categoryService.GetCategoryWithProductsAsync(id);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        [HttpGet("name/{name}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var category = await categoryService.GetCategoryByNameAsync(name);
            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        [HttpGet("{id}/has-active-products")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> HasActiveProducts(Guid id)
        {
            try
            {
                var hasProducts = await categoryService.HasActiveProductsAsync(id);
                return Ok(new { categoryId = id, hasActiveProducts = hasProducts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar produtos.", detail = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return BadRequest(new { message = "O nome da categoria é obrigatório." });

                var categoryId = await categoryService.CreateCategoryAsync(name);

                return CreatedAtRoute(nameof(GetCategoryById), new { id = categoryId }, new
                {
                    message = "Categoria criada com sucesso.",
                    categoryId
                });
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

        [HttpPut("{id}/name")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateName(Guid id, [FromBody] string newName)
        {
            try
            {
                await categoryService.UpdateCategoryNameAsync(id, newName);
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{categoryId}/products/{productId}")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct(Guid categoryId, Guid productId)
        {
            try
            {
                await categoryService.AddProductToCategoryAsync(categoryId, productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{categoryId}/products/{productId}")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveProduct(Guid categoryId, Guid productId)
        {
            try
            {
                await categoryService.RemoveProductFromCategoryAsync(categoryId, productId);
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

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await categoryService.DeleteCategoryAsync(id);
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

        [HttpPatch("{id}/restore")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await categoryService.RestoreCategoryAsync(id);
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
    }
}