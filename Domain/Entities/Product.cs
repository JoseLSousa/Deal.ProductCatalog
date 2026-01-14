namespace Domain.Entities
{
    public class Product(string Name, string Description, decimal Price, bool Active, Guid CategoryId)
    {
        public Guid ProductId { get; init; }
        public string Name { get; private set; } = Name;
        public string Description { get; private set; } = Description;
        public decimal Price { get; private set; } = Price;
        public bool Active { get; private set; } = Active;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public bool IsDeleted => DeletedAt.HasValue;

        public Guid CategoryId { get; private set; } = CategoryId;
        public Category Category { get; private set; } = default!;
        public ICollection<Tag> Tags { get; private set; } = [];

        private void ValidateNotDeleted()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Não é possível modificar um produto deletado.");
        }

        public void UpdateName(string newName)
        {
            ValidateNotDeleted();

            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(newName));

            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDescription(string newDescription)
        {
            ValidateNotDeleted();

            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("A descrição do produto não pode ser vazia.", nameof(newDescription));

            Description = newDescription;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePrice(decimal newPrice)
        {
            ValidateNotDeleted();

            if (newPrice < 0)
                throw new ArgumentException("O preço não pode ser negativo.", nameof(newPrice));

            Price = newPrice;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            ValidateNotDeleted();

            Active = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            ValidateNotDeleted();

            Active = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeCategory(Guid newCategoryId)
        {
            ValidateNotDeleted();

            if (newCategoryId == Guid.Empty)
                throw new ArgumentException("O ID da categoria não pode ser vazio.", nameof(newCategoryId));

            CategoryId = newCategoryId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddTag(Tag tag)
        {
            ValidateNotDeleted();

            ArgumentNullException.ThrowIfNull(tag);

            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void RemoveTag(Tag tag)
        {
            ValidateNotDeleted();

            ArgumentNullException.ThrowIfNull(tag);

            if (!Tags.Contains(tag))
            {
                return;
            }
            Tags.Remove(tag);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ClearTags()
        {
            ValidateNotDeleted();

            Tags.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("O produto já foi deletado.");

            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Restore()
        {
            if (!IsDeleted)
                throw new InvalidOperationException("O produto não está deletado.");

            DeletedAt = null;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
