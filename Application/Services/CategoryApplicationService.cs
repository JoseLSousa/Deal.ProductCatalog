using Application.Interfaces;
using Domain.Abstractions;
using Domain.Entities;

namespace Application.Services
{
    public class CategoryApplicationService(IUnitOfWork unitOfWork) : ICategoryApplicationService
    {
        public async Task<Guid> CreateCategoryAsync(string name)
        {
            var category = new Category(name);

            await unitOfWork.Categories.AddAsync(category);
            await unitOfWork.CommitAsync();

            return category.CategoryId;
        }

        public async Task UpdateCategoryNameAsync(Guid categoryId, string newName)
        {
            var category = await unitOfWork.Categories.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found.");

            category.UpdateName(newName);

            await unitOfWork.CommitAsync();
        }

        public async Task AddProductToCategoryAsync(Guid categoryId, Guid productId)
        {
            var category = await unitOfWork.Categories.GetWithProductsAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found.");

            var product = await unitOfWork.Products.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Product not found.");

            category.AddProduct(product);

            await unitOfWork.CommitAsync();
        }

        public async Task RemoveProductFromCategoryAsync(Guid categoryId, Guid productId)
        {
            var category = await unitOfWork.Categories.GetWithProductsAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found.");

            var product = category.Products.FirstOrDefault(p => p.ProductId == productId)
                ?? throw new KeyNotFoundException("Product not found in category.");

            category.RemoveProduct(product);

            await unitOfWork.CommitAsync();
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await unitOfWork.Categories.GetWithProductsAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found.");

            category.Delete();

            await unitOfWork.CommitAsync();
        }

        public async Task RestoreCategoryAsync(Guid categoryId)
        {
            var category = await unitOfWork.Categories.GetByIdAsync(categoryId)
                ?? throw new KeyNotFoundException("Category not found.");

            category.Restore();

            await unitOfWork.CommitAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId)
        {
            return await unitOfWork.Categories.GetByIdAsync(categoryId);
        }

        public async Task<Category?> GetCategoryByNameAsync(string name)
        {
            return await unitOfWork.Categories.GetByNameAsync(name);
        }

        public async Task<Category?> GetCategoryWithProductsAsync(Guid categoryId)
        {
            return await unitOfWork.Categories.GetWithProductsAsync(categoryId);
        }

        public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync()
        {
            return await unitOfWork.Categories.GetActiveAsync();
        }

        public async Task<IReadOnlyList<Category>> GetCategoriesWithActiveProductsAsync()
        {
            return await unitOfWork.Categories.GetWithActiveProductsAsync();
        }

        public async Task<bool> HasActiveProductsAsync(Guid categoryId)
        {
            return await unitOfWork.Categories.HasActiveProductsAsync(categoryId);
        }
    }
}
