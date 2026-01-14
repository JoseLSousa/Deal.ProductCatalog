using Application.DTOs;
using Domain.Entities;
using FluentAssertions;
using Infra.Data;
using Infra.Repositories;
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
        public async Task CreateTag_ShouldAddTagToDatabase()
        {
            // Arrange
            var tagDto = new TagDto("Tag Teste");

            // Act
            await _repository.CreateTag(tagDto);
            await _context.SaveChangesAsync();

            // Assert
            var tag = await _context.Tags.FirstOrDefaultAsync();
            tag.Should().NotBeNull();
            tag!.Name.Should().Be("Tag Teste");
        }

        [Fact]
        public async Task GetTagById_WhenTagExists_ShouldReturnTag()
        {
            // Arrange
            var tag = new Tag("Tag Teste");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTagById(tag.TagId);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Tag Teste");
        }

        [Fact]
        public async Task GetTagById_WhenTagNotExists_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _repository.GetTagById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ListAllTags_ShouldReturnAllNonDeletedTags()
        {
            // Arrange
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            var tag3 = new Tag("Tag 3");
            tag3.Delete();

            await _context.Tags.AddRangeAsync(tag1, tag2, tag3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllTags(includeDeleted: false);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Name == "Tag 1");
            result.Should().Contain(t => t.Name == "Tag 2");
        }

        [Fact]
        public async Task ListAllTags_WithIncludeDeleted_ShouldReturnAllTags()
        {
            // Arrange
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            tag2.Delete();

            await _context.Tags.AddRangeAsync(tag1, tag2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ListAllTags(includeDeleted: true);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetDeletedTags_ShouldReturnOnlyDeletedTags()
        {
            // Arrange
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            tag2.Delete();

            await _context.Tags.AddRangeAsync(tag1, tag2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetDeletedTags();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("Tag 2");
        }

        [Fact]
        public async Task GetTagsByProduct_ShouldReturnTagsForProduct()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag1 = new Tag("Tag 1");
            var tag2 = new Tag("Tag 2");
            var tag3 = new Tag("Tag 3");

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            tag1.AssignToProduct(product.ProductId);
            tag2.AssignToProduct(product.ProductId);
            tag3.AssignToProduct(product.ProductId);
            tag3.Delete();

            await _context.Tags.AddRangeAsync(tag1, tag2, tag3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTagsByProduct(product.ProductId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Name == "Tag 1");
            result.Should().Contain(t => t.Name == "Tag 2");
        }

        [Fact]
        public async Task UpdateTag_ShouldUpdateTagName()
        {
            // Arrange
            var tag = new Tag("Tag Original");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            var updateDto = new TagDto("Tag Atualizada");

            // Act
            await _repository.UpdateTag(tag.TagId, updateDto);

            // Assert
            var updatedTag = await _context.Tags.FindAsync(tag.TagId);
            updatedTag!.Name.Should().Be("Tag Atualizada");
        }

        [Fact]
        public async Task UpdateTag_WhenNotFound_ShouldThrowException()
        {
            // Arrange
            var updateDto = new TagDto("Tag");

            // Act
            Func<Task> act = async () => await _repository.UpdateTag(Guid.NewGuid(), updateDto);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Tag com id:* não encontrada!");
        }

        [Fact]
        public async Task DeleteTag_ShouldSoftDeleteTag()
        {
            // Arrange
            var tag = new Tag("Tag");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteTag(tag.TagId);

            // Assert
            var deletedTag = await _context.Tags.FindAsync(tag.TagId);
            deletedTag!.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTag_WhenNotFound_ShouldThrowException()
        {
            // Act
            Func<Task> act = async () => await _repository.DeleteTag(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Tag com id:* não encontrada!");
        }

        [Fact]
        public async Task RestoreTag_ShouldRestoreDeletedTag()
        {
            // Arrange
            var tag = new Tag("Tag");
            tag.Delete();
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            await _repository.RestoreTag(tag.TagId);

            // Assert
            var restoredTag = await _context.Tags.FindAsync(tag.TagId);
            restoredTag!.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task RestoreTag_WhenNotFound_ShouldThrowException()
        {
            // Act
            Func<Task> act = async () => await _repository.RestoreTag(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Tag com id:* não encontrada!");
        }

        [Fact]
        public async Task AssignTagToProduct_ShouldAssignTag()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            var tag = new Tag("Tag");

            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            await _repository.AssignTagToProduct(tag.TagId, product.ProductId);

            // Assert
            var updatedTag = await _context.Tags.FindAsync(tag.TagId);
            updatedTag!.ProductId.Should().Be(product.ProductId);

            var updatedProduct = await _context.Products
                .Include(p => p.Tags)
                .FirstAsync(p => p.ProductId == product.ProductId);
            updatedProduct.Tags.Should().Contain(tag);
        }

        [Fact]
        public async Task AssignTagToProduct_WhenTagNotFound_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            await _context.Categories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.AssignTagToProduct(Guid.NewGuid(), product.ProductId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Tag com id:* não encontrada!");
        }

        [Fact]
        public async Task AssignTagToProduct_WhenProductNotFound_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag");
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _repository.AssignTagToProduct(tag.TagId, Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Produto com id:* não encontrado!");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
