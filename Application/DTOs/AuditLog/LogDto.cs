using Domain.Enums;

namespace Application.DTOs.AuditLog
{
    public record LogDto(LogAction Action, string UserId, object Payload);
}
