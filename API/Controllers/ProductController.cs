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
    public class ProductController(IProductApplicationService productService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetAll()
        {
            var products = await productService.GetActiveProductsAsync();
            return Ok(products);
        }

        [HttpGet("search")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest(new { message = "Termo de busca é obrigatório." });

                var products = await productService.SearchProductsByNameAsync(searchTerm);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar produtos.", detail = ex.Message });
            }
        }

        [HttpGet("price-range")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0 || minPrice > maxPrice)
                    return BadRequest(new { message = "Intervalo de preço inválido." });

                var products = await productService.GetProductsByPriceRangeAsync(minPrice, maxPrice);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar produtos.", detail = ex.Message });
            }
        }

        [HttpGet("category/{categoryId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            try
            {
                var products = await productService.GetProductsByCategoryAsync(categoryId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar produtos.", detail = ex.Message });
            }
        }

        [HttpGet("{id}", Name = nameof(GetById))]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        [HttpGet("{id}/with-category")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetWithCategory(Guid id)
        {
            var product = await productService.GetProductWithCategoryAsync(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        [HttpGet("{id}/with-tags")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetWithTags(Guid id)
        {
            var product = await productService.GetProductWithTagsAsync(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        [HttpGet("{id}/complete")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetComplete(Guid id)
        {
            var product = await productService.GetProductWithCategoryAndTagsAsync(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RequestProductDto productDto)
        {
            try
            {
                var productId = await productService.CreateProductAsync(
                    productDto.Name,
                    productDto.Description,
                    productDto.Price,
                    productDto.Active,
                    productDto.CategoryId
                );

                return CreatedAtRoute(nameof(GetById), new { id = productId }, new
                {
                    message = "Produto criado com sucesso.",
                    productId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar produto.", detail = ex.Message });
            }
        }

        [HttpPut("{id}/name")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateName(Guid id, [FromBody] string newName)
        {
            try
            {
                await productService.UpdateProductNameAsync(id, newName);
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

        [HttpPatch("{id}/price")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] decimal newPrice)
        {
            try
            {
                await productService.UpdateProductPriceAsync(id, newPrice);
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

        [HttpPatch("{id}/description")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] string newDescription)
        {
            try
            {
                await productService.UpdateProductDescriptionAsync(id, newDescription);
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

        [HttpDelete("{id}")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await productService.DeleteProductAsync(id);
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
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await productService.RestoreProductAsync(id);
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

        [HttpPatch("{id}/activate")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                await productService.ActivateProductAsync(id);
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

        [HttpPatch("{id}/deactivate")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            try
            {
                await productService.DeactivateProductAsync(id);
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

        [HttpPatch("{productId}/category/{newCategoryId}")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeCategory(Guid productId, Guid newCategoryId)
        {
            try
            {
                await productService.ChangeCategoryAsync(productId, newCategoryId);
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

        [HttpPost("{productId}/tags/{tagId}")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddTag(Guid productId, Guid tagId)
        {
            try
            {
                await productService.AddTagToProductAsync(productId, tagId);
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

        [HttpDelete("{productId}/tags/{tagId}")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveTag(Guid productId, Guid tagId)
        {
            try
            {
                await productService.RemoveTagFromProductAsync(productId, tagId);
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

        [HttpDelete("{productId}/tags")]
        [Authorize(Policy = Policies.CanDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearTags(Guid productId)
        {
            try
            {
                await productService.ClearProductTagsAsync(productId);
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