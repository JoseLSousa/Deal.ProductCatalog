namespace Domain.Entities
{
    public class Tag(string Name)
    {
        public Guid TagId { get; init; }
        public string Name { get; private set; } = Name;
        public Guid ProductId { get; private set; }
        public Product Product { get; private set; } = default!;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted => DeletedAt.HasValue;

        private void ValidateNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Não é possível modificar uma tag deletada.");
        }

        public void UpdateName(string newName)
        {
            ValidateNotDeleted();

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome da tag não pode ser vazio.", nameof(newName));

            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignToProduct(Guid productId)
        {
            ValidateNotDeleted();

            if (productId == Guid.Empty)
                throw new ArgumentException("O ID do produto não pode ser vazio.", nameof(productId));

            ProductId = productId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("A tag já foi deletada.");

            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new InvalidOperationException("A tag não está deletada.");

            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
