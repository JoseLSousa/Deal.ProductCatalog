using Domain.Entities;

namespace Application.Interfaces
{
    public interface IProductApplicationService
    {
        // Comandos - Criação
        Task<Guid> CreateProductAsync(string name, string description, decimal price, bool active, Guid categoryId);

        // Comandos - Atualização
        Task UpdateProductNameAsync(Guid productId, string newName);
        Task UpdateProductDescriptionAsync(Guid productId, string newDescription);
        Task UpdateProductPriceAsync(Guid productId, decimal newPrice);
        Task ChangeCategoryAsync(Guid productId, Guid newCategoryId);

        // Comandos - Ativação/Desativação
        Task ActivateProductAsync(Guid productId);
        Task DeactivateProductAsync(Guid productId);

        // Comandos - Tags
        Task AddTagToProductAsync(Guid productId, Guid tagId);
        Task RemoveTagFromProductAsync(Guid productId, Guid tagId);
        Task ClearProductTagsAsync(Guid productId);

        // Comandos - Deleção/Restauração
        Task DeleteProductAsync(Guid productId);
        Task RestoreProductAsync(Guid productId);

        // Consultas
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task<Product?> GetProductWithCategoryAsync(Guid productId);
        Task<Product?> GetProductWithTagsAsync(Guid productId);
        Task<Product?> GetProductWithCategoryAndTagsAsync(Guid productId);
        Task<IReadOnlyList<Product>> GetActiveProductsAsync();
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<IReadOnlyList<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IReadOnlyList<Product>> SearchProductsByNameAsync(string searchTerm);
    }
}
