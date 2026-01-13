namespace Domain.Entities
{
    public class Category
    {
        public Guid CategoryId { get; init; }
        public string Name { get; private set; } = default!;

        public ICollection<Product> Products { get; private set; } = [];
    }
}
