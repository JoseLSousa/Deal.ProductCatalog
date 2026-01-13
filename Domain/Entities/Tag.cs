namespace Domain.Entities
{
    public class Tag(string Name)
    {
        public Guid TagId { get; init; }
        public string Name { get; private set; } = Name;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
