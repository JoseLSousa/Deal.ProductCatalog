using Application.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TagController(ITagApplicationService tagService) : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetAll()
        {
            var tags = await tagService.GetActiveTagsAsync();
            return Ok(tags);
        }

        [HttpGet("{id}", Name = nameof(GetTagById))]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetTagById(Guid id)
        {
            var tag = await tagService.GetTagByIdAsync(id);
            if (tag == null)
                return NotFound(new { message = "Tag não encontrada." });

            return Ok(tag);
        }

        [HttpGet("name/{name}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var tag = await tagService.GetTagByNameAsync(name);
            if (tag == null)
                return NotFound(new { message = "Tag não encontrada." });

            return Ok(tag);
        }

        [HttpGet("product/{productId}")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            try
            {
                var tags = await tagService.GetTagsByProductIdAsync(productId);
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar tags.", detail = ex.Message });
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
                    return BadRequest(new { message = "O nome da tag é obrigatório." });

                var tagId = await tagService.CreateTagAsync(name);

                return CreatedAtRoute(nameof(GetTagById), new { id = tagId }, new
                {
                    message = "Tag criada com sucesso.",
                    tagId
                });
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

        [HttpPut("{id}/name")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateName(Guid id, [FromBody] string newName)
        {
            try
            {
                await tagService.UpdateTagNameAsync(id, newName);
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

        [HttpPatch("{tagId}/product/{productId}")]
        [Authorize(Policy = Policies.CanWrite)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignToProduct(Guid tagId, Guid productId)
        {
            try
            {
                await tagService.AssignTagToProductAsync(tagId, productId);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await tagService.DeleteTagAsync(id);
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
                await tagService.RestoreTagAsync(id);
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