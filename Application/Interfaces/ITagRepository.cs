using Application.DTOs;

namespace Application.Interfaces
{
    public interface ITagRepository
    {
        // Consultas
        Task<List<TagDto>> ListAllTags(bool includeDeleted = false);
        Task<TagDto?> GetTagById(Guid id);
        Task<List<TagDto>> GetTagsByProduct(Guid productId);
        Task<List<TagDto>> GetDeletedTags();

        // Comandos básicos
        Task CreateTag(TagDto tagDto);
        Task UpdateTag(Guid id, TagDto tagDto);

        // Soft delete
        Task DeleteTag(Guid id);
        Task RestoreTag(Guid id);

        // Relacionamento
        Task AssignTagToProduct(Guid tagId, Guid productId);
    }
}