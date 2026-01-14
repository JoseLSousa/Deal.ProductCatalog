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
    public class ReportsController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IAuditLogService _auditLogService;

        public ReportsController(IExportService exportService, IAuditLogService auditLogService)
        {
            _exportService = exportService;
            _auditLogService = auditLogService;
        }

        [HttpGet("items/csv")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

                var csvBytes = await _exportService.GenerateCsvReportAsync();

                await _auditLogService.LogAsync(new Application.DTOs.LogDto(
                    LogAction.EXPORT_GENERATED,
                    userId,
                    new
                    {
                        Format = "CSV",
                        Timestamp = DateTime.UtcNow
                    }
                ));

                return File(csvBytes, "text/csv", $"produtos_relatorio_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao gerar relatório CSV.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("items/json")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> ExportJson()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

                var json = await _exportService.GenerateJsonReportAsync();

                await _auditLogService.LogAsync(new Application.DTOs.LogDto(
                    LogAction.EXPORT_GENERATED,
                    userId,
                    new
                    {
                        Format = "JSON",
                        Timestamp = DateTime.UtcNow
                    }
                ));

                return File(
                    System.Text.Encoding.UTF8.GetBytes(json),
                    "application/json",
                    $"produtos_relatorio_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao gerar relatório JSON.",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("items")]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Editor},{Roles.Viewer}")]
        public async Task<IActionResult> GetReportData()
        {
            try
            {
                var reportData = await _exportService.GetReportDataAsync();
                return Ok(reportData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro ao obter dados do relatório.",
                    detail = ex.Message
                });
            }
        }
    }
}
