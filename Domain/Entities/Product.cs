namespace Domain.Entities
{
    public class Product(string Name,string Description, decimal Price, bool Active, Guid CategoryId)
    {
        public Guid ProductId { get; init; }
        public string Name { get; private set; } = Name;
        public string Description { get; private set; } = Description;
        public decimal Price { get; private set; } = Price;
        public bool Active { get; private set; } = Active;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; }

        public Guid CategoryId { get; private set; } = CategoryId;
        public Category Category { get; private set; } = default!;
        public ICollection<Tag> Tags { get; private set; } = [];
        public void UpdateName(string newName)
        {
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
