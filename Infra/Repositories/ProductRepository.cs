using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ProductRepository(AppDbContext context, IAuditLogService logService) : IProductRepository
    {
        public async Task CreateProduct(ProductDto productDto)
        {
            await context.Products.AddAsync(new Product(
                    productDto.Name,
                    productDto.Description,
                    productDto.Price,
                    productDto.Active,
                    productDto.CategoryId
                    ));
            await context.SaveChangesAsync();
            await logService.LogAsync(new LogDto(
                "ITEM_CREATED",
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
            var itemExists = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                    ?? throw new Exception("Item not found");

            context.Products.Remove(itemExists);
            await context.SaveChangesAsync();
        }

        public async Task<ProductDto?> GetProductById(Guid id)
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

        public async Task UpdateProduct(Guid id, ProductDto itemDto)
        {
            var itemExists = await context.Products.FirstOrDefaultAsync(i => i.ProductId == id)
                ?? throw new Exception("Item not found");

            itemExists.UpdateName(itemDto.Name);
            context.Products.Update(itemExists);

            await context.SaveChangesAsync();
        }
    }
}
