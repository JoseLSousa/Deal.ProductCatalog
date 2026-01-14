using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class CategoryRepository(AppDbContext appDbContext) : ICategoryRepository
    {
        public async Task CreateCategory(CategoryDto categoryDto)
        {
            await appDbContext.Categories.AddAsync(
                new Category(categoryDto.Name));
            await appDbContext.SaveChangesAsync();
        }

        public async Task DeleteCategory(Guid id)
        {
            var categoryExists = await appDbContext
                .Categories.FirstOrDefaultAsync(c => c.CategoryId == id)
                ?? throw new KeyNotFoundException("Category not found");

            categoryExists.Delete();
            await appDbContext.SaveChangesAsync();
        }

        public async Task<CategoryDto?> GetCategoryById(Guid id)
        {
            return await appDbContext.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto(c.Name))
                .FirstOrDefaultAsync();
        }

        public async Task<List<CategoryDto>> ListAllCategories(bool includeDeleted)
        {
            var query = appDbContext.Categories.AsNoTracking();
            if (!includeDeleted)
                query = query.Where(c => c.DeletedAt == null);

            return await query
                .Select(c => new CategoryDto(c.Name))
                .ToListAsync();
        }

        public async Task<List<CategoryDto>> GetDeletedCategories()
        {
            return await appDbContext.Categories
                .AsNoTracking()
                .Where(c => c.DeletedAt != null)
                .Select(c => new CategoryDto(c.Name))
                .ToListAsync();
        }

        public async Task UpdateCategory(Guid id, CategoryDto categoryDto)
        {
            var categoryExists = await appDbContext
                .Categories.FirstOrDefaultAsync(c => c.CategoryId == id)
                ?? throw new KeyNotFoundException("Category not found");

            categoryExists.UpdateName(categoryDto.Name);
            await appDbContext.SaveChangesAsync();
        }

        public async Task RestoreCategory(Guid id)
        {
            var category = await appDbContext.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id)
                ?? throw new KeyNotFoundException("Category not found");

            category.Restore();
            await appDbContext.SaveChangesAsync();
        }

        public async Task AddProductToCategory(Guid categoryId, Guid productId)
        {
            var category = await appDbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId)
                ?? throw new KeyNotFoundException("Category not found");

            var product = await appDbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId)
                ?? throw new KeyNotFoundException("Product not found");

            category.AddProduct(product);
            await appDbContext.SaveChangesAsync();
        }

        public async Task RemoveProductFromCategory(Guid categoryId, Guid productId)
        {
            var category = await appDbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId)
                ?? throw new KeyNotFoundException("Category not found");

            var product = await appDbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId)
                ?? throw new KeyNotFoundException("Product not found");

            category.RemoveProduct(product);
            await appDbContext.SaveChangesAsync();
        }
    }
}
