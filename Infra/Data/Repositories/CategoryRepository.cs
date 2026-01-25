using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public class CategoryRepository(AppDbContext context) : Repository<Category>(context), ICategoryRepository
    {
        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeCategoryId = null)
        {
            return await DbSet
                .Where(c => !c.IsDeleted)
                .Where(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                .Where(c => !excludeCategoryId.HasValue || c.CategoryId != excludeCategoryId.Value)
                .AnyAsync();
        }

        public async Task<IReadOnlyList<Category>> GetActiveAsync()
        {
            return await DbSet
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await DbSet
                .Where(c => !c.IsDeleted)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IReadOnlyList<Category>> GetWithActiveProductsAsync()
        {
            return await DbSet
                .Include(c => c.Products)
                .Where(c => !c.IsDeleted)
                .Where(c => c.Products.Any(p => p.Active && !p.IsDeleted))
                .ToListAsync();
        }

        public async Task<Category?> GetWithProductsAsync(Guid categoryId)
        {
            return await DbSet
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId && !c.IsDeleted);
        }

        public async Task<bool> HasActiveProductsAsync(Guid categoryId)
        {
            return await DbSet
                .Where(c => c.CategoryId == categoryId && !c.IsDeleted)
                .AnyAsync(c => c.Products.Any(p => !p.IsDeleted));
        }
    }
}
