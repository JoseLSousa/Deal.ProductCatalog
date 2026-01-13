using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class ItemRepository(AppDbContext context, IAuditLogService logService) : IItemRepository
    {
        public async Task CreateItem(ItemDto itemDto)
        {
            await context.Products.AddAsync(new Product(itemDto.Name));
            await context.SaveChangesAsync();
            await logService.LogAsync(new LogDto(
                "ITEM_CREATED",
                Guid.NewGuid().ToString(),
                itemDto.Name
                ));
        }

        public async Task DeleteItem(Guid id)
        {
            var itemExists = await context.Products.FirstOrDefaultAsync(i => i.ItemId == id)
                    ?? throw new Exception("Item not found");

            context.Products.Remove(itemExists);
            await context.SaveChangesAsync();
        }

        public async Task<ItemDto?> GetItemById(Guid id)
        {
            return await context.Products
                .AsNoTracking()
                .Select(i => new ItemDto(
                    i.Name
                    ))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ItemDto>> ListAllProducts()
        {
            return await context.Products
                .AsNoTracking()
                .Select(i => new ItemDto(
                    i.Name
                    ))
                .ToListAsync();
        }

        public async Task UpdateItem(Guid id, ItemDto itemDto)
        {
            var itemExists = await context.Products.FirstOrDefaultAsync(i => i.ItemId == id)
                ?? throw new Exception("Item not found");

            itemExists.UpdateName(itemDto.Name);
            context.Products.Update(itemExists);

            await context.SaveChangesAsync();
        }
    }
}
