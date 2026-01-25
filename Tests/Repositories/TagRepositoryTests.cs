using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly TagRepository _repository;

        public TagRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new TagRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTagToDatabase()
        {
            // Arrange
            var tag = new Tag("Tag Teste");

            // Act
            await _repository.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Tags.FirstOrDefaultAsync();
            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Tag Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WhenTagExists_ShouldReturnTag()
        {
            // Arrange
            var tag = new Tag("Tag Teste");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(tag.TagId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Tag Teste");
        }

        [Fact]
        public async Task GetByIdAsync_WhenTagNotExists_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnMatchingTag()
        {
            // Arrange
            var tag = new Tag("Tag 1");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameAsync("Tag 1");

            // Assert
            result.Should().NotBeNull();
            result!.TagId.Should().Be(tag.TagId);
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnOnlyNonDeletedTags()
        {
            // Arrange
            var active = new Tag("Ativa");
            var deleted = new Tag("Deletada");
            deleted.Delete();
            await _context.Tags.AddRangeAsync(active, deleted);
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
            var tag = new Tag("Duplicada");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act & Assert
            (await _repository.ExistsWithNameAsync("Duplicada")).Should().BeTrue();
            (await _repository.ExistsWithNameAsync("Duplicada", tag.TagId)).Should().BeFalse();
        }

        [Fact]
        public async Task GetByProductIdAsync_ShouldReturnTagsForProduct()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            var tagDeleted = new Tag("Tag 3");

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            tag1.AssignToProduct(product.ProductId);
            tag2.AssignToProduct(product.ProductId);
            tagDeleted.AssignToProduct(product.ProductId);
            tagDeleted.Delete();

            await _context.Tags.AddRangeAsync(tag1, tag2, tagDeleted);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByProductIdAsync(product.ProductId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Name == "Tag 1");
            result.Should().Contain(t => t.Name == "Tag 2");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
