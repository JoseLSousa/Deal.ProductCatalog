using Domain.Entities;

namespace Application.Interfaces
{
    public interface ICategoryApplicationService
    {
        // Comandos - Criação
        Task<Guid> CreateCategoryAsync(string name);

        // Comandos - Atualização
        Task UpdateCategoryNameAsync(Guid categoryId, string newName);

        // Comandos - Gerenciamento de Produtos
        Task AddProductToCategoryAsync(Guid categoryId, Guid productId);
        Task RemoveProductFromCategoryAsync(Guid categoryId, Guid productId);

        // Comandos - Deleção/Restauração
        Task DeleteCategoryAsync(Guid categoryId);
        Task RestoreCategoryAsync(Guid categoryId);

        // Consultas
        Task<Category?> GetCategoryByIdAsync(Guid categoryId);
        Task<Category?> GetCategoryByNameAsync(string name);
        Task<Category?> GetCategoryWithProductsAsync(Guid categoryId);
        Task<IReadOnlyList<Category>> GetActiveCategoriesAsync();
        Task<IReadOnlyList<Category>> GetCategoriesWithActiveProductsAsync();
        Task<bool> HasActiveProductsAsync(Guid categoryId);
    }
}
