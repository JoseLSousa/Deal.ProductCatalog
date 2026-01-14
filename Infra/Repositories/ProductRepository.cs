using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ProductRepository(AppDbContext context, IAuditLogService logService) : IProductRepository
    {
        public async Task CreateProduct(ProductDto productDto)
        {
            var categoryExists = await context.Categories
                .AnyAsync(c => c.CategoryId == productDto.CategoryId);

            if (!categoryExists)
            {
                throw new KeyNotFoundException($"Categoria com id:{productDto.CategoryId} não encontrada!");
            }

            await context.Products.AddAsync(new Product(
                    productDto.Name,
                    productDto.Description,
                    productDto.Price,
                    productDto.Active,
                    productDto.CategoryId
                    ));
            await context.SaveChangesAsync();
            await logService.LogAsync(new LogDto(
                LogAction.PRODUCT_CREATED,
                Guid.NewGuid().ToString(),
                new
                {
                    productDto.Name,
                    productDto.Description,
                    productDto.Price,
                    productDto.Active,
                    productDto.CategoryId
                }
                ));
        }

        public async Task DeleteProduct(Guid id)
        {
            var productExists = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                    ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            productExists.Delete();
            await context.SaveChangesAsync();
        }

        public async Task<ProductDto?> GetProductById(Guid id)
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                    ))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ProductDto>> ListAllProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                    ))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> ListAllProducts(bool includeDeleted)
        {
            var query = context.Products.AsNoTracking();
            if (!includeDeleted)
            {
                query = query.Where(p => p.DeletedAt == null);
            }
            return await query
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetProductsByCategory(Guid categoryId)
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.DeletedAt == null)
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetDeletedProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.DeletedAt != null)
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetActiveProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.Active && p.DeletedAt == null)
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ProductDto>> GetInactiveProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => !p.Active && p.DeletedAt == null)
                .Select(i => new ProductDto(
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task UpdateProduct(Guid id, ProductDto productDto)
        {
            var productExists = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            productExists.UpdateName(productDto.Name);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductPrice(Guid id, decimal newPrice)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            product.UpdatePrice(newPrice);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductDescription(Guid id, string newDescription)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            product.UpdateDescription(newDescription);
            await context.SaveChangesAsync();
        }

        public async Task RestoreProduct(Guid id)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            product.Restore();
            await context.SaveChangesAsync();
        }

        public async Task ActivateProduct(Guid id)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            product.Activate();
            await context.SaveChangesAsync();
        }

        public async Task DeactivateProduct(Guid id)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new KeyNotFoundException($"Produto com id:{id} não encontrado!");

            product.Deactivate();
            await context.SaveChangesAsync();
        }

        public async Task ChangeProductCategory(Guid productId, Guid newCategoryId)
        {
            var product = await context.Products.FirstOrDefaultAsync(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException($"Produto com id:{productId} não encontrado!");

            product.ChangeCategory(newCategoryId);
            await context.SaveChangesAsync();
        }

        public async Task AddTagToProduct(Guid productId, Guid tagId)
        {
            var product = await context.Products.Include(p => p.Tags).FirstOrDefaultAsync(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException($"Produto com id:{productId} não encontrado!");

            var tag = await context.Tags.FirstOrDefaultAsync(t => t.TagId == tagId)
                ?? throw new KeyNotFoundException($"Tag com id:{tagId} não encontrada!");

            product.AddTag(tag);
            await context.SaveChangesAsync();
        }

        public async Task RemoveTagFromProduct(Guid productId, Guid tagId)
        {
            var product = await context.Products.Include(p => p.Tags).FirstOrDefaultAsync(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException($"Produto com id:{productId} não encontrado!");

            var tag = await context.Tags.FirstOrDefaultAsync(t => t.TagId == tagId)
                ?? throw new KeyNotFoundException($"Tag com id:{tagId} não encontrada!");

            product.RemoveTag(tag);
            await context.SaveChangesAsync();
        }

        public async Task ClearProductTags(Guid productId)
        {
            var product = await context.Products.Include(p => p.Tags).FirstOrDefaultAsync(i => i.ProductId == productId)
                ?? throw new KeyNotFoundException($"Produto com id:{productId} não encontrado!");

            product.ClearTags();
            await context.SaveChangesAsync();
        }
    }
}
