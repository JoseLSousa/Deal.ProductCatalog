using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetWithCategoryAsync(Guid productId);
        Task<IReadOnlyList<Product>> GetActiveAsync();
        Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId);
    }
}
