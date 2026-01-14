using Application.DTOs;
using Application.DTOs.Search;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ProductRepository(AppDbContext context, IAuditLogService logService) : IProductRepository
    {
        public async Task CreateProduct(RequestProductDto productDto)
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

        public async Task<ResponseProductDto?> GetProductById(Guid id)
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.ProductId == id)
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                    ))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ResponseProductDto>> ListAllProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                    ))
                .ToListAsync();
        }

        public async Task<List<ResponseProductDto>> ListAllProducts(bool includeDeleted)
        {
            var query = context.Products.AsNoTracking();
            if (!includeDeleted)
            {
                query = query.Where(p => p.DeletedAt == null);
            }
            return await query
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ResponseProductDto>> GetProductsByCategory(Guid categoryId)
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.CategoryId == categoryId && p.DeletedAt == null)
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ResponseProductDto>> GetDeletedProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.DeletedAt != null)
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ResponseProductDto>> GetActiveProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => p.Active && p.DeletedAt == null)
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task<List<ResponseProductDto>> GetInactiveProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Where(p => !p.Active && p.DeletedAt == null)
                .Select(i => new ResponseProductDto(
                    i.ProductId,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.Active,
                    i.CategoryId
                ))
                .ToListAsync();
        }

        public async Task UpdateProduct(Guid id, RequestProductDto productDto)
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

        public async Task<ProductSearchResultDto> SearchProductsAsync(ProductSearchDto searchDto)
        {
            var query = context.Products
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Filtro por termo (busca em Nome e Descrição)
            if (!string.IsNullOrWhiteSpace(searchDto.Term))
            {
                var term = searchDto.Term.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(term) || 
                    p.Description.ToLower().Contains(term));
            }

            // Filtro por Categoria
            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == searchDto.CategoryId.Value);
            }

            // Filtro por Preço Mínimo
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= searchDto.MinPrice.Value);
            }

            // Filtro por Preço Máximo
            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= searchDto.MaxPrice.Value);
            }

            // Filtro por Status Ativo
            if (searchDto.Active.HasValue)
            {
                query = query.Where(p => p.Active == searchDto.Active.Value);
            }

            // Filtro por Tags
            if (searchDto.Tags != null && searchDto.Tags.Count != 0)
            {
                query = query.Where(p => p.Tags.Any(t => searchDto.Tags.Contains(t.Name)));
            }

            // Ordenação
            query = searchDto.SortBy?.ToLower() switch
            {
                "name" => searchDto.SortDescending 
                    ? query.OrderByDescending(p => p.Name) 
                    : query.OrderBy(p => p.Name),
                "price" => searchDto.SortDescending 
                    ? query.OrderByDescending(p => p.Price) 
                    : query.OrderBy(p => p.Price),
                "date" => searchDto.SortDescending 
                    ? query.OrderByDescending(p => p.CreatedAt) 
                    : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            // Calcular total antes da paginação
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)searchDto.PageSize);

            // Calcular agregações
            var averagePrice = await query.AnyAsync() ? await query.AverageAsync(p => p.Price) : 0;
            
            var itemsByCategory = await query
                .GroupBy(p => p.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);

            // Paginação
            var items = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(p => new ResponseProductDto(
                    p.ProductId,
                    p.Name,
                    p.Description,
                    p.Price,
                    p.Active,
                    p.CategoryId
                ))
                .ToListAsync();

            return new ProductSearchResultDto
            {
                Items = items,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CurrentPage = searchDto.Page,
                PageSize = searchDto.PageSize,
                AveragePrice = averagePrice,
                ItemsByCategory = itemsByCategory
            };
        }
    }
}
