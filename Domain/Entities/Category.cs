namespace Domain.Entities
{
    public class Category(string Name)
    {
        public Guid CategoryId { get; init; }
        public string Name { get; private set; } = Name;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted => DeletedAt.HasValue;

        public ICollection<Product> Products { get; private set; } = [];

        private void ValidateNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Não é possível modificar uma categoria deletada.");
        }

        public void UpdateName(string newName)
        {
            ValidateNotDeleted();

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome da categoria não pode ser vazio.", nameof(newName));

            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddProduct(Product product)
        {
            ValidateNotDeleted();

            ArgumentNullException.ThrowIfNull(product);

            if (!Products.Contains(product))
            {
                Products.Add(product);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void RemoveProduct(Product product)
        {
            ValidateNotDeleted();

            ArgumentNullException.ThrowIfNull(product);

            if (!Products.Contains(product))
            {
                return;
            }
            Products.Remove(product);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("A categoria já foi deletada.");

            if (Products.Any(p => !p.IsDeleted))
                throw new InvalidOperationException("Não é possível deletar uma categoria com produtos ativos.");

            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new InvalidOperationException("A categoria não está deletada.");

            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
