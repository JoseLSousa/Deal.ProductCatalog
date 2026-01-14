using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Repositories
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<IAuditLogService> _mockAuditService;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _mockAuditService = new Mock<IAuditLogService>();
            _repository = new ProductRepository(_context, _mockAuditService.Object);
        }

        [Fact]
        public async Task CreateProduct_ShouldAddProductToDatabase()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var productDto = new ProductDto("Produto Teste", "Descrição", 10.00m, true, category.CategoryId);

            // Act
            await _repository.CreateProduct(productDto);

            // Assert
            _context.ChangeTracker.Clear();
            var product = await _context.Products.FirstOrDefaultAsync();
            product.Should().NotBeNull();
            product!.Name.Should().Be("Produto Teste");
            product.Description.Should().Be("Descrição");
            product.Price.Should().Be(10.00m);
            _mockAuditService.Verify(s => s.LogAsync(It.IsAny<LogDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_WhenCategoryNotFound_ShouldThrowException()
        {
            // Arrange
            var nonExistentCategoryId = Guid.NewGuid();
            var productDto = new ProductDto("Produto Teste", "Descrição", 10.00m, true, nonExistentCategoryId);

            // Act
            Func<Task> act = async () => await _repository.CreateProduct(productDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Categoria com id:{nonExistentCategoryId} não encontrada!");
        }

        [Fact]
        public async Task GetProductById_WhenProductExists_ShouldReturnProduct()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto Teste", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetProductById(product.ProductId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Produto Teste");
        }

        [Fact]
        public async Task GetProductById_WhenProductNotExists_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetProductById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ListAllProducts_ShouldReturnAllNonDeletedProducts()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição 1", 10.00m, true, category.CategoryId);
            var product2 = new Product("Produto 2", "Descrição 2", 20.00m, true, category.CategoryId);
            var product3 = new Product("Produto 3", "Descrição 3", 30.00m, true, category.CategoryId);
            product3.Delete();

            await _context.Products.AddRangeAsync(product1, product2, product3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllProducts(includeDeleted: false);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Produto 1");
            result.Should().Contain(p => p.Name == "Produto 2");
        }

        [Fact]
        public async Task ListAllProducts_WithIncludeDeleted_ShouldReturnAllProducts()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição 1", 10.00m, true, category.CategoryId);
            var product2 = new Product("Produto 2", "Descrição 2", 20.00m, true, category.CategoryId);
            product2.Delete();

            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllProducts(includeDeleted: true);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProductsByCategory_ShouldReturnProductsInCategory()
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
            var result = await _repository.GetProductsByCategory(category1.CategoryId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(p => p.CategoryId == category1.CategoryId);
        }

        [Fact]
        public async Task GetDeletedProducts_ShouldReturnOnlyDeletedProducts()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product1 = new Product("Produto 1", "Descrição", 10.00m, true, category.CategoryId);
            var product2 = new Product("Produto 2", "Descrição", 20.00m, true, category.CategoryId);
            product2.Delete();

            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDeletedProducts();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Produto 2");
        }

        [Fact]
        public async Task GetActiveProducts_ShouldReturnOnlyActiveProducts()
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
            var result = await _repository.GetActiveProducts();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Produto 1");
        }

        [Fact]
        public async Task GetInactiveProducts_ShouldReturnOnlyInactiveProducts()
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
            var result = await _repository.GetInactiveProducts();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Produto 2");
        }

        [Fact]
        public async Task UpdateProduct_ShouldUpdateProductName()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto Original", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var updateDto = new ProductDto("Produto Atualizado", "Descrição", 10.00m, true, category.CategoryId);

            // Act
            await _repository.UpdateProduct(product.ProductId, updateDto);

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.ProductId);
            updatedProduct!.Name.Should().Be("Produto Atualizado");
        }

        [Fact]
        public async Task UpdateProduct_WhenNotFound_ShouldThrowException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var updateDto = new ProductDto("Produto", "Descrição", 10.00m, true, categoryId);
            var nonExistentId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _repository.UpdateProduct(nonExistentId, updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Produto com id:{nonExistentId} não encontrado!");
        }

        [Fact]
        public async Task UpdateProductPrice_ShouldUpdatePrice()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateProductPrice(product.ProductId, 25.00m);

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.ProductId);
            updatedProduct!.Price.Should().Be(25.00m);
        }

        [Fact]
        public async Task UpdateProductDescription_ShouldUpdateDescription()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição Original", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateProductDescription(product.ProductId, "Nova Descrição");

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.ProductId);
            updatedProduct!.Description.Should().Be("Nova Descrição");
        }

        [Fact]
        public async Task DeleteProduct_ShouldSoftDeleteProduct()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteProduct(product.ProductId);

            // Assert
            var deletedProduct = await _context.Products.FindAsync(product.ProductId);
            deletedProduct.Should().NotBeNull();
            deletedProduct!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenNotFound_ShouldThrowException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _repository.DeleteProduct(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Produto com id:{nonExistentId} não encontrado!");
        }

        [Fact]
        public async Task RestoreProduct_ShouldRestoreDeletedProduct()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            product.Delete();
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.RestoreProduct(product.ProductId);

            // Assert
            var restoredProduct = await _context.Products.FindAsync(product.ProductId);
            restoredProduct!.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task ActivateProduct_ShouldSetActiveToTrue()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, false, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.ActivateProduct(product.ProductId);

            // Assert
            var activatedProduct = await _context.Products.FindAsync(product.ProductId);
            activatedProduct!.Active.Should().BeTrue();
        }

        [Fact]
        public async Task DeactivateProduct_ShouldSetActiveToFalse()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeactivateProduct(product.ProductId);

            // Assert
            var deactivatedProduct = await _context.Products.FindAsync(product.ProductId);
            deactivatedProduct!.Active.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeProductCategory_ShouldUpdateCategory()
        {
            // Arrange
            var oldCategory = new Category("Categoria Antiga");
            var newCategory = new Category("Categoria Nova");
            await _context.Categories.AddRangeAsync(oldCategory, newCategory);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, oldCategory.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.ChangeProductCategory(product.ProductId, newCategory.CategoryId);

            // Assert
            var updatedProduct = await _context.Products.FindAsync(product.ProductId);
            updatedProduct!.CategoryId.Should().Be(newCategory.CategoryId);
        }

        [Fact]
        public async Task AddTagToProduct_ShouldAddTag()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag = new Tag("Tag Teste");
            await _context.Products.AddAsync(product);
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AddTagToProduct(product.ProductId, tag.TagId);

            // Assert
            var updatedProduct = await _context.Products
                .Include(p => p.Tags)
                .FirstAsync(p => p.ProductId == product.ProductId);
            updatedProduct.Tags.Should().Contain(tag);
        }

        [Fact]
        public async Task AddTagToProduct_WhenProductNotFound_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag Teste");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            var nonExistentProductId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _repository.AddTagToProduct(nonExistentProductId, tag.TagId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Produto com id:{nonExistentProductId} não encontrado!");
        }

        [Fact]
        public async Task AddTagToProduct_WhenTagNotFound_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var nonExistentTagId = Guid.NewGuid();

            // Act
            Func<Task> act = async () => await _repository.AddTagToProduct(product.ProductId, nonExistentTagId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"Tag com id:{nonExistentTagId} não encontrada!");
        }

        [Fact]
        public async Task RemoveTagFromProduct_ShouldRemoveTag()
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
            await _repository.RemoveTagFromProduct(product.ProductId, tag.TagId);

            // Assert
            var updatedProduct = await _context.Products
                .Include(p => p.Tags)
                .FirstAsync(p => p.ProductId == product.ProductId);
            updatedProduct.Tags.Should().NotContain(tag);
        }

        [Fact]
        public async Task ClearProductTags_ShouldRemoveAllTags()
        {
            // Arrange
            var category = new Category("Categoria Teste");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            product.AddTag(new Tag("Tag 1"));
            product.AddTag(new Tag("Tag 2"));
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.ClearProductTags(product.ProductId);

            // Assert
            var updatedProduct = await _context.Products
                .Include(p => p.Tags)
                .FirstAsync(p => p.ProductId == product.ProductId);
            updatedProduct.Tags.Should().BeEmpty();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
