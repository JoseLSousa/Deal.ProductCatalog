using Domain.Enums;

namespace Application.DTOs
{
    public record LogDto(LogAction Action, string UserId, object Payload);
}
