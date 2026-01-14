using Domain.Enums;

namespace Domain.Entities
{
    public class AuditLog(LogAction action, string userId, object payload)
    {
        public int LogId { get; set; }
        public LogAction Action { get; set; } = action;
        public string UserId { get; set; } = userId;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public object Payload { get; set; } = payload;
    }
}
