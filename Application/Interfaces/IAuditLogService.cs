using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(LogDto logDto);
    }
}
