using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITagApplicationService
    {
        // Comandos - Criação
        Task<Guid> CreateTagAsync(string name);

        // Comandos - Atualização
        Task UpdateTagNameAsync(Guid tagId, string newName);
        Task AssignTagToProductAsync(Guid tagId, Guid productId);

        // Comandos - Deleção/Restauração
        Task DeleteTagAsync(Guid tagId);
        Task RestoreTagAsync(Guid tagId);

        // Consultas
        Task<Tag?> GetTagByIdAsync(Guid tagId);
        Task<Tag?> GetTagByNameAsync(string name);
        Task<IReadOnlyList<Tag>> GetTagsByProductIdAsync(Guid productId);
        Task<IReadOnlyList<Tag>> GetActiveTagsAsync();
    }
}
