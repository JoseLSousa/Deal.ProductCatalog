namespace Domain.Entities
{
    public class AuditLog(string action, string userId, object payload)
    {
        public int LogId { get; set; }
        public string Action { get; set; } = action;
        public string UserId { get; set; } = userId;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public object Payload { get; set; } = payload;
    }
}
