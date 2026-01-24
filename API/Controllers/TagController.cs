using Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController(ITagRepository tagRepository) : ControllerBase
    {
        // GET: api/tag
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeDeleted = false)
        {
            var tags = await tagRepository.ListAllTags(includeDeleted);
            return Ok(tags);
        }

        // GET: api/tag/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tag = await tagRepository.GetTagById(id);
            if (tag == null)
                return NotFound(new { message = "Tag não encontrada." });

            return Ok(tag);
        }

        // GET: api/tag/deleted
        [HttpGet("deleted")]
        public async Task<IActionResult> GetDeleted()
        {
            var tags = await tagRepository.GetDeletedTags();
            return Ok(tags);
        }

        // GET: api/tag/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var tags = await tagRepository.GetTagsByProduct(productId);
            return Ok(tags);
        }

        // POST: api/tag
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagDto tagDto)
        {
            try
            {
                await tagRepository.CreateTag(tagDto);
                return Ok(new { message = "Tag criada com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao criar tag.", detail = ex.Message });
            }
        }

        // PUT: api/tag/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TagDto tagDto)
        {
            try
            {
                await tagRepository.UpdateTag(id, tagDto);
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

        // DELETE: api/tag/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await tagRepository.DeleteTag(id);
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

        // POST: api/tag/{id}/restore
        [HttpPost("{id}/restore")]
        public async Task<IActionResult> Restore(Guid id)
        {
            try
            {
                await tagRepository.RestoreTag(id);
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

        // POST: api/tag/{tagId}/product/{productId}
        [HttpPost("{tagId}/product/{productId}")]
        public async Task<IActionResult> AssignToProduct(Guid tagId, Guid productId)
        {
            try
            {
                await tagRepository.AssignTagToProduct(tagId, productId);
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