using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.NoSql;

namespace Infra.Services
{
    public class AuditLogService(AuditDbContext context) : IAuditLogService
    {
        public async Task LogAsync(LogDto logDto)
        {
            var log = new AuditLog
                (
                logDto.Action,
                logDto.UserId,
                logDto.Payload
                );
            try
            {
                await context.AuditLogs.InsertOneAsync(log);
            }
            catch (Exception ex)
            {
                // Tratar exceções conforme necessário
                throw new Exception("Erro ao salvar o log de auditoria.", ex);
            }
        }
    }
}
