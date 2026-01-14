using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        // Consultas
        Task<List<ProductDto>> ListAllProducts(bool includeDeleted = false);
        Task<ProductDto?> GetProductById(Guid id);
        Task<List<ProductDto>> GetProductsByCategory(Guid categoryId);
        Task<List<ProductDto>> GetDeletedProducts();
        Task<List<ProductDto>> GetActiveProducts();
        Task<List<ProductDto>> GetInactiveProducts();

        // Comandos básicos
        Task CreateProduct(ProductDto productDto);
        Task UpdateProduct(Guid id, ProductDto productDto);
        Task UpdateProductPrice(Guid id, decimal newPrice);
        Task UpdateProductDescription(Guid id, string newDescription);

        // Soft delete
        Task DeleteProduct(Guid id);
        Task RestoreProduct(Guid id);

        // Controle de estado
        Task ActivateProduct(Guid id);
        Task DeactivateProduct(Guid id);

        // Categoria
        Task ChangeProductCategory(Guid productId, Guid newCategoryId);

        // Tags
        Task AddTagToProduct(Guid productId, Guid tagId);
        Task RemoveTagFromProduct(Guid productId, Guid tagId);
        Task ClearProductTags(Guid productId);
    }
}
