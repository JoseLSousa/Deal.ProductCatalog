using API.Controllers;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class TagControllerTests
    {
        private readonly Mock<ITagApplicationService> _mockService;
        private readonly TagController _controller;

        public TagControllerTests()
        {
            _mockService = new Mock<ITagApplicationService>();
            _controller = new TagController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithActiveTags()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new("Tag 1"),
                new("Tag 2")
            };
            _mockService.Setup(s => s.GetActiveTagsAsync()).ReturnsAsync(tags.AsReadOnly());

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public async Task GetTagById_WhenTagExists_ShouldReturnOkWithTag()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tag = new Tag("Tag");
            _mockService.Setup(s => s.GetTagByIdAsync(tagId)).ReturnsAsync(tag);

            // Act
            var result = await _controller.GetTagById(tagId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tag);
        }

        [Fact]
        public async Task GetTagById_WhenTagNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.GetTagByIdAsync(tagId)).ReturnsAsync((Tag?)null);

            // Act
            var result = await _controller.GetTagById(tagId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByName_WhenTagExists_ShouldReturnOkWithTag()
        {
            // Arrange
            var tagName = "Tag Teste";
            var tag = new Tag(tagName);
            _mockService.Setup(s => s.GetTagByNameAsync(tagName)).ReturnsAsync(tag);

            // Act
            var result = await _controller.GetByName(tagName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetByName_WhenTagNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagName = "Tag Inexistente";
            _mockService.Setup(s => s.GetTagByNameAsync(tagName)).ReturnsAsync((Tag?)null);

            // Act
            var result = await _controller.GetByName(tagName);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByProduct_ShouldReturnTagsForProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tags = new List<Tag> { new("Tag do Produto") };
            _mockService.Setup(s => s.GetTagsByProductIdAsync(productId)).ReturnsAsync(tags.AsReadOnly());

            // Act
            var result = await _controller.GetByProduct(productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.GetTagsByProductIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtRoute()
        {
            // Arrange
            var tagName = "Nova Tag";
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.CreateTagAsync(tagName)).ReturnsAsync(tagId);

            // Act
            var result = await _controller.Create(tagName);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();
            _mockService.Verify(s => s.CreateTagAsync(tagName), Times.Once);
        }

        [Fact]
        public async Task Create_WithEmptyName_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.Create("");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateName_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var newName = "Tag Atualizada";
            _mockService.Setup(s => s.UpdateTagNameAsync(tagId, newName)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateName(tagId, newName);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.UpdateTagNameAsync(tagId, newName), Times.Once);
        }

        [Fact]
        public async Task UpdateName_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var newName = "Tag Atualizada";
            _mockService.Setup(s => s.UpdateTagNameAsync(tagId, newName))
                .ThrowsAsync(new KeyNotFoundException("Tag não encontrada."));

            // Act
            var result = await _controller.UpdateName(tagId, newName);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task AssignToProduct_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.AssignTagToProductAsync(tagId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignToProduct(tagId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.AssignTagToProductAsync(tagId, productId), Times.Once);
        }

        [Fact]
        public async Task AssignToProduct_WhenTagNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.AssignTagToProductAsync(tagId, productId))
                .ThrowsAsync(new KeyNotFoundException("Tag não encontrada."));

            // Act
            var result = await _controller.AssignToProduct(tagId, productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteTagAsync(tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.DeleteTagAsync(tagId), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteTagAsync(tagId))
                .ThrowsAsync(new KeyNotFoundException("Tag não encontrada."));

            // Act
            var result = await _controller.Delete(tagId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.RestoreTagAsync(tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.RestoreTagAsync(tagId), Times.Once);
        }

        [Fact]
        public async Task Restore_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.RestoreTagAsync(tagId))
                .ThrowsAsync(new KeyNotFoundException("Tag não encontrada."));

            // Act
            var result = await _controller.Restore(tagId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
