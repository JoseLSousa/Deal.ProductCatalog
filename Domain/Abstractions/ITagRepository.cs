using Domain.Entities;

namespace Domain.Abstractions
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag?> GetByNameAsync(string name);
        Task<IReadOnlyList<Tag>> GetByProductIdAsync(Guid productId);
        Task<IReadOnlyList<Tag>> GetActiveAsync();
        Task<bool> ExistsWithNameAsync(string name, Guid? excludeTagId = null);
    }
}
