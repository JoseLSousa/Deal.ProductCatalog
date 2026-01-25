using Domain.Entities;

namespace Domain.Abstractions
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<Category?> GetWithProductsAsync(Guid categoryId);
        Task<IReadOnlyList<Category>> GetActiveAsync();
        Task<IReadOnlyList<Category>> GetWithActiveProductsAsync();
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeCategoryId = null);
        Task<bool> HasActiveProductsAsync(Guid categoryId);
    }
}
