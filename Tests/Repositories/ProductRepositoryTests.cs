using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddProductToDatabase()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto Teste", "Descrição", 10.00m, true, category.CategoryId);

            // Act
            await _repository.AddAsync(product);
            await _context.SaveChangesAsync();

            // Assert
            var result = await _context.Products.FirstOrDefaultAsync();
            result.Should().NotBeNull();
            result!.Name.Should().Be("Produto Teste");
            result.Description.Should().Be("Descrição");
            result.Price.Should().Be(10.00m);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto Teste", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Produto Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductNotExists_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetByIdAsync(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnOnlyActiveProducts()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição", 10.00m, true, category.CategoryId);
            var product2 = new Product("Produto 2", "Descrição", 20.00m, false, category.CategoryId);

            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetActiveAsync();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Produto 1");
        }

        [Fact]
        public async Task GetByCategoryAsync_ShouldReturnProductsInCategory()
        {
            // Arrange
            var category1 = new Category("Categoria 1");
            var category2 = new Category("Categoria 2");
            await _context.Categories.AddRangeAsync(category1, category2);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição", 10.00m, true, category1.CategoryId);
            var product2 = new Product("Produto 2", "Descrição", 20.00m, true, category1.CategoryId);
            var product3 = new Product("Produto 3", "Descrição", 30.00m, true, category2.CategoryId);

            await _context.Products.AddRangeAsync(product1, product2, product3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCategoryAsync(category1.CategoryId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.CategoryId == category1.CategoryId);
        }

        [Fact]
        public async Task GetByPriceRangeAsync_ShouldReturnProductsInRange()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição", 10.00m, true, category.CategoryId);
            var product2 = new Product("Produto 2", "Descrição", 20.00m, true, category.CategoryId);
            var product3 = new Product("Produto 3", "Descrição", 30.00m, true, category.CategoryId);

            await _context.Products.AddRangeAsync(product1, product2, product3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPriceRangeAsync(15.00m, 25.00m);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Produto 2");
        }

        [Fact]
        public async Task GetWithCategoryAsync_ShouldReturnProductWithCategory()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWithCategoryAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Category.Should().NotBeNull();
            result.Category!.Name.Should().Be("Categoria Teste");
        }

        [Fact]
        public async Task GetWithTagsAsync_ShouldReturnProductWithTags()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            product.AddTag(tag1);
            product.AddTag(tag2);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWithTagsAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetWithCategoryAndTagsAsync_ShouldReturnProductWithCategoryAndTags()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag = new Tag("Tag Teste");
            product.AddTag(tag);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetWithCategoryAndTagsAsync(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Category.Should().NotBeNull();
            result.Tags.Should().HaveCount(1);
        }

        [Fact]
        public async Task SearchByNameAsync_ShouldReturnProductsMatchingSearchTerm()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Notebook Dell", "Descrição", 1000.00m, true, category.CategoryId);
            var product2 = new Product("Notebook Lenovo", "Descrição", 800.00m, true, category.CategoryId);
            var product3 = new Product("Mouse Logitech", "Descrição", 50.00m, true, category.CategoryId);

            await _context.Products.AddRangeAsync(product1, product2, product3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchByNameAsync("Notebook");

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(p => p.Name.Should().Contain("Notebook"));
        }

        [Fact]
        public async Task ExistsWithNameAsync_WhenNameExists_ShouldReturnTrue()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto Único", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsWithNameAsync("Produto Único");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsWithNameAsync_WhenNameNotExists_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.ExistsWithNameAsync("Produto Inexistente");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsWithNameAsync_WhenExcludingProductId_ShouldNotCountExcludedProduct()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsWithNameAsync("Produto", product.ProductId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Update_ShouldMarkProductAsModified()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            _context.Categories.Add(category);
            _context.SaveChanges();

            var product = new Product("Produto Original", "Descrição", 10.00m, true, category.CategoryId);
            _context.Products.Add(product);
            _context.SaveChanges();

            var updatedProduct = _context.Products.Find(product.ProductId)!;
            updatedProduct.UpdateName("Produto Atualizado");
            updatedProduct.UpdateDescription("Nova Descrição");
            updatedProduct.UpdatePrice(15.00m);

            // Act
            _repository.Update(updatedProduct);

            // Assert
            _context.SaveChanges();
            var result = _context.Products.Find(product.ProductId);
            result!.Name.Should().Be("Produto Atualizado");
            result.Description.Should().Be("Nova Descrição");
            result.Price.Should().Be(15.00m);
        }

        [Fact]
        public void Remove_ShouldMarkProductAsDeleted()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            _context.Categories.Add(category);
            _context.SaveChanges();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            _context.Products.Add(product);
            _context.SaveChanges();

            // Act
            _repository.Remove(product);
            _context.SaveChanges();

            // Assert
            var result = _context.Products.Find(product.ProductId);
            result.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
