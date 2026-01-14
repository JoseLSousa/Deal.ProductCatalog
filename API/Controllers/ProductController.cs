using Application.DTOs;
using Application.DTOs.Search;
using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController(IProductRepository productRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var products = await productRepository.ListAllProducts(includeDeleted);
            return Ok(products);
        }

        [HttpGet("search")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> Search([FromQuery] ProductSearchDto searchDto)
        {
            try
            {
                var result = await productRepository.SearchProductsAsync(searchDto);
                return Ok(result);
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
            var product = await productRepository.GetProductById(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        [HttpPost]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] RequestProductDto productDto)
        {
            try
            {
                await productRepository.CreateProduct(productDto);
                
                // Em um cenário real, CreateProduct deveria retornar o ID do produto criado
                // Por enquanto, retornamos 201 Created sem Location header
                return StatusCode(StatusCodes.Status201Created, new 
                { 
                    message = "Produto criado com sucesso.",
                    product = productDto
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar produto.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] RequestProductDto productDto)
        {
            try
            {
                await productRepository.UpdateProduct(id, productDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.UpdateProductPrice(id, newPrice);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.UpdateProductDescription(id, newDescription);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.DeleteProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.RestoreProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.ActivateProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.DeactivateProduct(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.ChangeProductCategory(productId, newCategoryId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.AddTagToProduct(productId, tagId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.RemoveTagFromProduct(productId, tagId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
                await productRepository.ClearProductTags(productId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}