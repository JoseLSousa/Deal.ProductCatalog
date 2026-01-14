using Application.DTOs;
using Application.DTOs.Search;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        // Consultas
        Task<List<ResponseProductDto>> ListAllProducts(bool includeDeleted = false);
        Task<ResponseProductDto?> GetProductById(Guid id);
        Task<List<ResponseProductDto>> GetProductsByCategory(Guid categoryId);
        Task<List<ResponseProductDto>> GetDeletedProducts();
        Task<List<ResponseProductDto>> GetActiveProducts();
        Task<List<ResponseProductDto>> GetInactiveProducts();
        
        // Busca avançada
        Task<ProductSearchResultDto> SearchProductsAsync(ProductSearchDto searchDto);

        // Comandos básicos
        Task CreateProduct(RequestProductDto productDto);
        Task UpdateProduct(Guid id, RequestProductDto productDto);
        Task UpdateProductPrice(Guid id, decimal newPrice);
        Task UpdateProductDescription(Guid id, string newDescription);

        // Soft delete
        Task DeleteProduct(Guid id);
        Task RestoreProduct(Guid id);

        // Controle de estado
        Task ActivateProduct(Guid id);
        Task DeactivateProduct(Guid id);

        // Categoria
        Task ChangeProductCategory(Guid productId, Guid newCategoryId);

        // Tags
        Task AddTagToProduct(Guid productId, Guid tagId);
        Task RemoveTagFromProduct(Guid productId, Guid tagId);
        Task ClearProductTags(Guid productId);
    }
}
