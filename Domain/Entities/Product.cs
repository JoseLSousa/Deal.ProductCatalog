namespace Domain.Entities
{
    public class Product(string Name)
    {
        public Guid ProductId { get; init; }
        public string Name { get; private set; } = Name;
        public decimal Price { get; private set; }
        public bool Active { get; private set; }

        public Guid CategoryId { get; private set; }
        public Category Category { get; private set; } = default!;
        public void UpdateName(string newName) => Name = newName;
    }
}
