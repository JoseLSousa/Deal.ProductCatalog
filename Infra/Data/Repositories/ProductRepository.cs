using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public sealed class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
    {
        public async Task<IReadOnlyList<Product>> GetActiveAsync()
        {
            return await DbSet.
                Where(p => p.Active && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await DbSet.
                Where(p =>
                p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product?> GetWithCategoryAsync(Guid productId)
        {
            return await DbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p =>
                p.ProductId == productId
                && !p.IsDeleted);
        }
    }
}
