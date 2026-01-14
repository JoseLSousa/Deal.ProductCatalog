using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepository productRepository) : ControllerBase
    {
        // GET: api/product
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var products = await productRepository.ListAllProducts(includeDeleted);
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await productRepository.GetProductById(id);
            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        // GET: api/product/category/{categoryId}
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            var products = await productRepository.GetProductsByCategory(categoryId);
            return Ok(products);
        }

        // GET: api/product/deleted
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var products = await productRepository.GetDeletedProducts();
            return Ok(products);
        }

        // GET: api/product/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var products = await productRepository.GetActiveProducts();
            return Ok(products);
        }

        // GET: api/product/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            var products = await productRepository.GetInactiveProducts();
            return Ok(products);
        }

        // POST: api/product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        {
            try
            {
                await productRepository.CreateProduct(productDto);
                // Nota: Idealmente, deveria retornar o ID real do produto criado
                return Ok(new { message = "Produto criado com sucesso." });
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

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductDto productDto)
        {
            await productRepository.UpdateProduct(id, productDto);
            return NoContent();
        }

        // PATCH: api/product/{id}/price
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] decimal newPrice)
        {
            await productRepository.UpdateProductPrice(id, newPrice);
            return NoContent();
        }

        // PATCH: api/product/{id}/description
        [HttpPatch("{id}/description")]
        public async Task<IActionResult> UpdateDescription(Guid id, [FromBody] string newDescription)
        {
            await productRepository.UpdateProductDescription(id, newDescription);
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await productRepository.DeleteProduct(id);
            return NoContent();
        }

        // POST: api/product/{id}/restore
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            await productRepository.RestoreProduct(id);
            return NoContent();
        }

        // POST: api/product/{id}/activate
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(Guid id)
        {
            await productRepository.ActivateProduct(id);
            return NoContent();
        }

        // POST: api/product/{id}/deactivate
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            await productRepository.DeactivateProduct(id);
            return NoContent();
        }

        // PATCH: api/product/{productId}/category/{newCategoryId}
        [HttpPatch("{productId}/category/{newCategoryId}")]
        public async Task<IActionResult> ChangeCategory(Guid productId, Guid newCategoryId)
        {
            await productRepository.ChangeProductCategory(productId, newCategoryId);
            return NoContent();
        }

        // POST: api/product/{productId}/tags/{tagId}
        [HttpPost("{productId}/tags/{tagId}")]
        public async Task<IActionResult> AddTag(Guid productId, Guid tagId)
        {
            await productRepository.AddTagToProduct(productId, tagId);
            return NoContent();
        }

        // DELETE: api/product/{productId}/tags/{tagId}
        [HttpDelete("{productId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTag(Guid productId, Guid tagId)
        {
            await productRepository.RemoveTagFromProduct(productId, tagId);
            return NoContent();
        }

        // DELETE: api/product/{productId}/tags
        [HttpDelete("{productId}/tags")]
        public async Task<IActionResult> ClearTags(Guid productId)
        {
            await productRepository.ClearProductTags(productId);
            return NoContent();
        }
    }
}