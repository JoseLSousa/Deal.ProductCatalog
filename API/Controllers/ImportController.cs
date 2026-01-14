using Application.DTOs.Import;
using Application.Interfaces;
using Domain.Constants;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImportController : ControllerBase
    {
        private readonly IImportService _importService;
        private readonly IAuditLogService _auditLogService;

        public ImportController(IImportService importService, IAuditLogService auditLogService)
        {
            _importService = importService;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        [Authorize(Policy = Policies.CanWrite)]
        public async Task<IActionResult> ImportProducts()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

                var result = await _importService.ImportFromExternalApiAsync(userId);

                await _auditLogService.LogAsync(new Application.DTOs.LogDto(
                    LogAction.IMPORT_EXECUTED,
                    userId,
                    new
                    {
                        TotalFetched = result.TotalFetched,
                        Imported = result.Imported,
                        Skipped = result.Skipped,
                        Timestamp = DateTime.UtcNow
                    }
                ));

                return Ok(new
                {
                    message = "Importação concluída.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao importar produtos.",
                    detail = ex.Message
                });
            }
        }
    }
}
