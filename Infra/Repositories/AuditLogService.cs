using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.NoSql;

namespace Infra.Repositories
{
    public class AuditLogService(AuditDbContext context) : IAuditLogService
    {
        public async Task LogAsync(LogDto logDto)
        {
            var log = new AuditLog(
                logDto.Action,
                logDto.UserId,
                logDto.Payload
                );

            await context.AuditLogs.InsertOneAsync(log);
        }
    }
}
