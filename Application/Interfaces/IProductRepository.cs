using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDto>> ListAllProducts();
        Task<ProductDto?> GetProductById(Guid id);
        Task CreateProduct(ProductDto productDto);
        Task DeleteProduct(Guid id);
        Task UpdateProduct(Guid id, ProductDto itemDto);
    }
}
