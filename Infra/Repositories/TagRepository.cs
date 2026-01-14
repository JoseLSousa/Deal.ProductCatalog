using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Infra.Repositories
{
    public class TagRepository(
        AppDbContext context
        ) : ITagRepository
    {
        public async Task AssignTagToProduct(Guid tagId, Guid productId)
        {
            var tag = await context.Tags
                .FirstOrDefaultAsync(t => t.TagId == tagId)
                ?? throw new KeyNotFoundException($"Tag com id:{tagId} não encontrada!");

            var product = await context.Products
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.ProductId == productId)
                ?? throw new KeyNotFoundException($"Produto com id:{productId} não encontrado!");

            tag.AssignToProduct(productId);
            product.AddTag(tag);
            await context.SaveChangesAsync();
        }

        public async Task CreateTag(TagDto tagDto)
        {
            await context.Tags.AddAsync(new Tag(tagDto.Name));
            await context.SaveChangesAsync();
        }

        public async Task DeleteTag(Guid id)
        {
            var tagExists = await context.Tags.FirstOrDefaultAsync(t => t.TagId == id)
                ?? throw new KeyNotFoundException($"Tag com id:{id} não encontrada!");

            tagExists.Delete();
            await context.SaveChangesAsync();
        }

        public async Task<List<TagDto>> GetDeletedTags()
        {
            return await context.Tags
                .AsNoTracking()
                .Where(t => t.DeletedAt != null)
                .Select(t => new TagDto(t.Name))
                .ToListAsync();
        }

        public async Task<TagDto?> GetTagById(Guid id)
        {
            return await context.Tags
                .AsNoTracking()
                .Where(t => t.TagId == id)
                .Select(t => new TagDto(t.Name))
                .FirstOrDefaultAsync();
        }

        public async Task<List<TagDto>> GetTagsByProduct(Guid productId)
        {
            return await context.Tags
                .AsNoTracking()
                .Where(t => t.ProductId == productId && t.DeletedAt == null)
                .Select(t => new TagDto(t.Name))
                .ToListAsync();
        }

        public async Task<List<TagDto>> ListAllTags(bool includeDeleted = false)
        {
            var query = context.Tags.AsNoTracking();
            if (!includeDeleted)
                query = query.Where(t => t.DeletedAt == null);

            return await query
                .Select(t => new TagDto(t.Name))
                .ToListAsync();
        }

        public async Task RestoreTag(Guid id)
        {
            var tag = await context.Tags
                .FirstOrDefaultAsync(t => t.TagId == id)
                ?? throw new KeyNotFoundException($"Tag com id:{id} não encontrada!");

            tag.Restore();
            await context.SaveChangesAsync();
        }

        public async Task UpdateTag(Guid id, TagDto tagDto)
        {
            var tagExists = await context.Tags.FirstOrDefaultAsync(t => t.TagId == id)
                ?? throw new KeyNotFoundException($"Tag com id:{id} não encontrada!");

            tagExists.UpdateName(tagDto.Name);
            await context.SaveChangesAsync();
        }
    }
}
