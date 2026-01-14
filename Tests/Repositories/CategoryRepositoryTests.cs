using Application.DTOs;
using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories
{
    public class CategoryRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new CategoryRepository(_context);
        }

        [Fact]
        public async Task CreateCategory_ShouldAddCategoryToDatabase()
        {
            // Arrange
            var categoryDto = new CategoryDto("Categoria Teste");

            // Act
            await _repository.CreateCategory(categoryDto);
            await _context.SaveChangesAsync();

            // Assert
            var category = await _context.Categories.FirstOrDefaultAsync();
            category.Should().NotBeNull();
            category!.Name.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetCategoryById_WhenCategoryExists_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetCategoryById(category.CategoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetCategoryById_WhenCategoryNotExists_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetCategoryById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ListAllCategories_ShouldReturnAllNonDeletedCategories()
        {
            // Arrange
            var category1 = new Category("Categoria 1");
            var category2 = new Category("Categoria 2");
            var category3 = new Category("Categoria 3");
            category3.Delete();

            await _context.Categories.AddRangeAsync(category1, category2, category3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllCategories(includeDeleted: false);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(c => c.Name == "Categoria 1");
            result.Should().Contain(c => c.Name == "Categoria 2");
        }

        [Fact]
        public async Task ListAllCategories_WithIncludeDeleted_ShouldReturnAllCategories()
        {
            // Arrange
            var category1 = new Category("Categoria 1");
            var category2 = new Category("Categoria 2");
            category2.Delete();

            await _context.Categories.AddRangeAsync(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllCategories(includeDeleted: true);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetDeletedCategories_ShouldReturnOnlyDeletedCategories()
        {
            // Arrange
            var category1 = new Category("Categoria 1");
            var category2 = new Category("Categoria 2");
            category2.Delete();

            await _context.Categories.AddRangeAsync(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDeletedCategories();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Categoria 2");
        }

        [Fact]
        public async Task UpdateCategory_ShouldUpdateCategoryName()
        {
            // Arrange
            var category = new Category("Categoria Original");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var updateDto = new CategoryDto("Categoria Atualizada");

            // Act
            await _repository.UpdateCategory(category.CategoryId, updateDto);

            // Assert
            var updatedCategory = await _context.Categories.FindAsync(category.CategoryId);
            updatedCategory!.Name.Should().Be("Categoria Atualizada");
        }

        [Fact]
        public async Task UpdateCategory_WhenNotFound_ShouldThrowException()
        {
            // Arrange
            var updateDto = new CategoryDto("Categoria");

            // Act
            Func<Task> act = async () => await _repository.UpdateCategory(Guid.NewGuid(), updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found");
        }

        [Fact]
        public async Task RestoreCategory_ShouldRestoreDeletedCategory()
        {
            // Arrange
            var category = new Category("Categoria");
            category.Delete();
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            await _repository.RestoreCategory(category.CategoryId);

            // Assert
            var restoredCategory = await _context.Categories.FindAsync(category.CategoryId);
            restoredCategory!.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task RestoreCategory_WhenNotFound_ShouldThrowException()
        {
            // Act
            Func<Task> act = async () => await _repository.RestoreCategory(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found");
        }

        [Fact]
        public async Task AddProductToCategory_ShouldAddProduct()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AddProductToCategory(category.CategoryId, product.ProductId);

            // Assert
            var updatedCategory = await _context.Categories
                .Include(c => c.Products)
                .FirstAsync(c => c.CategoryId == category.CategoryId);
            updatedCategory.Products.Should().Contain(product);
        }

        [Fact]
        public async Task AddProductToCategory_WhenCategoryNotFound_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, Guid.NewGuid());
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.AddProductToCategory(Guid.NewGuid(), product.ProductId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Category not found");
        }

        [Fact]
        public async Task AddProductToCategory_WhenProductNotFound_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.AddProductToCategory(category.CategoryId, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Product not found");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
