using Application.DTOs;

namespace Application.Interfaces
{
    public interface ICategoryRepository
    {
        // Consultas
        Task<List<CategoryDto>> ListAllCategories(bool includeDeleted = false);
        Task<CategoryDto?> GetCategoryById(Guid id);
        Task<List<CategoryDto>> GetDeletedCategories();

        // Comandos básicos
        Task CreateCategory(CategoryDto categoryDto);
        Task UpdateCategory(Guid id, CategoryDto categoryDto);

        // Soft delete
        Task DeleteCategory(Guid id);
        Task RestoreCategory(Guid id);

        // Operações de relacionamento
        Task AddProductToCategory(Guid categoryId, Guid productId);
        Task RemoveProductFromCategory(Guid categoryId, Guid productId);
    }
}
