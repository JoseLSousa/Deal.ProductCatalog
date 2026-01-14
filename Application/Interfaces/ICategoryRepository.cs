using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICategoryRepository
    {
        // Consultas
        Task<List<ResponseCategoryDto>> ListAllCategories(bool includeDeleted = false);
        Task<ResponseCategoryDto?> GetCategoryById(Guid id);
        Task<List<ResponseCategoryDto>> GetDeletedCategories();

        // Comandos básicos
        Task CreateCategory(RequestCategoryDto categoryDto);
        Task UpdateCategory(Guid id, RequestCategoryDto categoryDto);

        // Soft delete
        Task DeleteCategory(Guid id);
        Task RestoreCategory(Guid id);

        // Operações de relacionamento
        Task AddProductToCategory(Guid categoryId, Guid productId);
        Task RemoveProductFromCategory(Guid categoryId, Guid productId);
    }
}
