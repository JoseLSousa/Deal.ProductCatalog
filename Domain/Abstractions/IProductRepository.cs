using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetWithCategoryAsync(Guid productId);
        Task<Product?> GetWithTagsAsync(Guid productId);
        Task<Product?> GetWithCategoryAndTagsAsync(Guid productId);
        Task<IReadOnlyList<Product>> GetActiveAsync();
        Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId);
        Task<IReadOnlyList<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm);
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeProductId = null);
    }
}
