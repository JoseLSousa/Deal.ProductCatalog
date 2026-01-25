using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class TagRepository(AppDbContext context) : Repository<Tag>(context), ITagRepository
    {
        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeTagId = null)
        {
            return await DbSet
                .Where(t => !t.IsDeleted)
                .Where(t => t.Name.ToLower() == name.ToLower())
                .Where(t => !excludeTagId.HasValue || t.TagId != excludeTagId.Value)
                .AnyAsync();
        }

        public async Task<IReadOnlyList<Tag>> GetActiveAsync()
        {
            return await DbSet
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await DbSet
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyList<Tag>> GetByProductIdAsync(Guid productId)
        {
            return await DbSet
                .Where(t => !t.IsDeleted)
                .Where(t => t.ProductId == productId)
                .ToListAsync();
        }
    }
}
