using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class TagControllerTests
    {
        private readonly Mock<ITagRepository> _mockRepository;
        private readonly TagController _controller;

        public TagControllerTests()
        {
            _mockRepository = new Mock<ITagRepository>();
            _controller = new TagController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithTags()
        {
            // Arrange
            var tags = new List<TagDto>
            {
                new("Tag 1"),
                new("Tag 2")
            };
            _mockRepository.Setup(r => r.ListAllTags(false)).ReturnsAsync(tags);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tags);
        }

        [Fact]
        public async Task GetAll_WithIncludeDeleted_ShouldCallRepositoryWithTrueParameter()
        {
            // Arrange
            _mockRepository.Setup(r => r.ListAllTags(true)).ReturnsAsync(new List<TagDto>());

            // Act
            await _controller.GetAll(includeDeleted: true);

            // Assert
            _mockRepository.Verify(r => r.ListAllTags(true), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenTagExists_ShouldReturnOkWithTag()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tag = new TagDto("Tag");
            _mockRepository.Setup(r => r.GetTagById(tagId)).ReturnsAsync(tag);

            // Act
            var result = await _controller.GetById(tagId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(tag);
        }

        [Fact]
        public async Task GetById_WhenTagNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetTagById(tagId)).ReturnsAsync((TagDto?)null);

            // Act
            var result = await _controller.GetById(tagId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetByProduct_ShouldReturnTagsForProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tags = new List<TagDto> { new("Tag do Produto") };
            _mockRepository.Setup(r => r.GetTagsByProduct(productId)).ReturnsAsync(tags);

            // Act
            var result = await _controller.GetByProduct(productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetTagsByProduct(productId), Times.Once);
        }

        [Fact]
        public async Task GetDeleted_ShouldReturnDeletedTags()
        {
            // Arrange
            var deletedTags = new List<TagDto> { new("Tag Deletada") };
            _mockRepository.Setup(r => r.GetDeletedTags()).ReturnsAsync(deletedTags);

            // Act
            var result = await _controller.GetDeleted();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetDeletedTags(), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnOk()
        {
            // Arrange
            var tagDto = new TagDto("Nova Tag");
            _mockRepository.Setup(r => r.CreateTag(tagDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(tagDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.CreateTag(tagDto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var tagDto = new TagDto("Tag Atualizada");
            _mockRepository.Setup(r => r.UpdateTag(tagId, tagDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(tagId, tagDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateTag(tagId, tagDto), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeleteTag(tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.DeleteTag(tagId), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RestoreTag(tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.RestoreTag(tagId), Times.Once);
        }

        [Fact]
        public async Task AssignToProduct_ShouldReturnNoContent()
        {
            // Arrange
            var tagId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.AssignTagToProduct(tagId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AssignToProduct(tagId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.AssignTagToProduct(tagId, productId), Times.Once);
        }
    }
}
