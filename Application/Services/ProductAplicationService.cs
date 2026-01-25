using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services
{
    public class ProductAplicationService(IUnitOfWork unitOfWork) : IProductApplicationService
    {
        public async Task<Guid> CreateProductAsync(string name, string description, decimal price, bool active, Guid categoryId)
        {
            var product = new Product(name, description, price, active, categoryId);

            await unitOfWork.Products.AddAsync(product);
            await unitOfWork.CommitAsync();

            return product.ProductId;
        }

        public async Task UpdateProductNameAsync(Guid productId, string newName)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.UpdateName(newName);

            await unitOfWork.CommitAsync();
        }

        public async Task UpdateProductDescriptionAsync(Guid productId, string newDescription)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.UpdateDescription(newDescription);

            await unitOfWork.CommitAsync();
        }

        public async Task UpdateProductPriceAsync(Guid productId, decimal newPrice)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.UpdatePrice(newPrice);

            await unitOfWork.CommitAsync();
        }

        public async Task ChangeCategoryAsync(Guid productId, Guid newCategoryId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.ChangeCategory(newCategoryId);

            await unitOfWork.CommitAsync();
        }

        public async Task ActivateProductAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Activate();

            await unitOfWork.CommitAsync();
        }

        public async Task DeactivateProductAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Deactivate();

            await unitOfWork.CommitAsync();
        }

        public async Task AddTagToProductAsync(Guid productId, Guid tagId)
        {
            var product = await unitOfWork.Products.GetWithTagsAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            var tag = await unitOfWork.Tags.GetByIdAsync(tagId)
                ?? throw new KeyNotFoundException("Tag not found.");

            product.AddTag(tag);

            await unitOfWork.CommitAsync();
        }

        public async Task RemoveTagFromProductAsync(Guid productId, Guid tagId)
        {
            var product = await unitOfWork.Products.GetWithTagsAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            var tag = product.Tags.FirstOrDefault(t => t.TagId == tagId)
                ?? throw new KeyNotFoundException("Tag not found in product.");

            product.RemoveTag(tag);

            await unitOfWork.CommitAsync();
        }

        public async Task ClearProductTagsAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.ClearTags();

            await unitOfWork.CommitAsync();
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Delete();

            await unitOfWork.CommitAsync();
        }

        public async Task RestoreProductAsync(Guid productId)
        {
            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            product.Restore();

            await unitOfWork.CommitAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await unitOfWork.Products.GetByIdAsync(productId);
        }

        public async Task<Product?> GetProductWithCategoryAsync(Guid productId)
        {
            return await unitOfWork.Products.GetWithCategoryAsync(productId);
        }

        public async Task<Product?> GetProductWithTagsAsync(Guid productId)
        {
            return await unitOfWork.Products.GetWithTagsAsync(productId);
        }

        public async Task<Product?> GetProductWithCategoryAndTagsAsync(Guid productId)
        {
            return await unitOfWork.Products.GetWithCategoryAndTagsAsync(productId);
        }

        public async Task<IReadOnlyList<Product>> GetActiveProductsAsync()
        {
            return await unitOfWork.Products.GetActiveAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            return await unitOfWork.Products.GetByCategoryAsync(categoryId);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await unitOfWork.Products.GetByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<IReadOnlyList<Product>> SearchProductsByNameAsync(string searchTerm)
        {
            return await unitOfWork.Products.SearchByNameAsync(searchTerm);
        }
    }
}
