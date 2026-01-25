using Domain.Abstractions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repositories
{
    public sealed class ProductRepository(AppDbContext context) : Repository<Product>(context), IProductRepository
    {
        public async Task<bool> ExistsWithNameAsync(string name, Guid? excludeProductId = null)
        {
            return await DbSet
                .Where(p => !p.IsDeleted)
                .Where(p => p.Name.ToLower() == name.ToLower())
                .Where(p => !excludeProductId.HasValue || p.ProductId != excludeProductId.Value)
                .AnyAsync();
        }

        public async Task<IReadOnlyList<Product>> GetActiveAsync()
        {
            return await DbSet
                .Where(p => p.Active && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await DbSet
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await DbSet
                .Where(p => !p.IsDeleted)
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        public async Task<Product?> GetWithCategoryAndTagsAsync(Guid productId)
        {
            return await DbSet
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        public async Task<Product?> GetWithCategoryAsync(Guid productId)
        {
            return await DbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        public async Task<Product?> GetWithTagsAsync(Guid productId)
        {
            return await DbSet
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.ProductId == productId && !p.IsDeleted);
        }

        public async Task<IReadOnlyList<Product>> SearchByNameAsync(string searchTerm)
        {
            return await DbSet
                .Where(p => !p.IsDeleted)
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }
    }
}
