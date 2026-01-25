using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Data.Repositories;
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
        public async Task AddAsync_ShouldAddCategoryToDatabase()
        {
            // Arrange
            var category = new Category("Categoria Teste");

            // Act
            await _repository.AddAsync(category);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Categories.FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(category.CategoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnMatchingCategory()
        {
            // Arrange
            var category = new Category("Categoria 1");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameAsync("Categoria 1");

            // Assert
            result.Should().NotBeNull();
            result!.CategoryId.Should().Be(category.CategoryId);
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnOnlyNonDeleted()
        {
            // Arrange
            var active = new Category("Ativa");
            var deleted = new Category("Deletada");
            deleted.Delete();
            await _context.Categories.AddRangeAsync(active, deleted);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Ativa");
        }

        [Fact]
        public async Task ExistsWithNameAsync_ShouldRespectExclusion()
        {
            // Arrange
            var c1 = new Category("Duplicada");
            await _context.Categories.AddAsync(c1);
            await _context.SaveChangesAsync();

            // Act & Assert
            (await _repository.ExistsWithNameAsync("Duplicada")).Should().BeTrue();
            (await _repository.ExistsWithNameAsync("Duplicada", c1.CategoryId)).Should().BeFalse();
        }

        [Fact]
        public async Task GetWithProductsAsync_ShouldIncludeProducts()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Prod", "Desc", 10m, true, category.CategoryId);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWithProductsAsync(category.CategoryId);

            // Assert
            result.Should().NotBeNull();
            result!.Products.Should().ContainSingle(p => p.ProductId == product.ProductId);
        }

        [Fact]
        public async Task GetWithActiveProductsAsync_ShouldReturnCategoriesHavingActiveProducts()
        {
            // Arrange
            var catWithActive = new Category("Com ativo");
            var activeProduct = new Product("Ativo", "Desc", 10m, true, catWithActive.CategoryId);
            var catWithoutActive = new Category("Sem ativo");
            var inactiveProduct = new Product("Inativo", "Desc", 5m, false, catWithoutActive.CategoryId);
            await _context.Categories.AddRangeAsync(catWithActive, catWithoutActive);
            await _context.Products.AddRangeAsync(activeProduct, inactiveProduct);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWithActiveProductsAsync();

            // Assert
            result.Should().ContainSingle(c => c.CategoryId == catWithActive.CategoryId);
            result.Should().NotContain(c => c.CategoryId == catWithoutActive.CategoryId);
        }

        [Fact]
        public async Task HasActiveProductsAsync_ShouldReturnTrueOnlyWhenActiveProductsExist()
        {
            // Arrange
            var category = new Category("Categoria");
            var active = new Product("Ativo", "Desc", 10m, true, category.CategoryId);
            var inactive = new Product("Inativo", "Desc", 5m, false, category.CategoryId);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddRangeAsync(active, inactive);
            await _context.SaveChangesAsync();

            // Act & Assert
            (await _repository.HasActiveProductsAsync(category.CategoryId)).Should().BeTrue();

            // Soft-delete active product to make none active
            active.Delete();
            await _context.SaveChangesAsync();

            (await _repository.HasActiveProductsAsync(category.CategoryId)).Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
