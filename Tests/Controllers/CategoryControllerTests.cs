using API.Controllers;
using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryRepository> _mockRepository;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockRepository = new Mock<ICategoryRepository>();
            _controller = new CategoryController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCategories()
        {
            // Arrange
            var categories = new List<ResponseCategoryDto>
            {
                new(Guid.NewGuid(), "Categoria 1"),
                new(Guid.NewGuid(), "Categoria 2")
            };
            _mockRepository.Setup(r => r.ListAllCategories(false)).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(categories);
        }

        [Fact]
        public async Task GetAll_WithIncludeDeleted_ShouldCallRepositoryWithTrueParameter()
        {
            // Arrange
            _mockRepository.Setup(r => r.ListAllCategories(true)).ReturnsAsync(new List<ResponseCategoryDto>());

            // Act
            await _controller.GetAll(includeDeleted: true);

            // Assert
            _mockRepository.Verify(r => r.ListAllCategories(true), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCategoryExists_ShouldReturnOkWithCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new ResponseCategoryDto(categoryId, "Categoria");
            _mockRepository.Setup(r => r.GetCategoryById(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetById(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(category);
        }

        [Fact]
        public async Task GetById_WhenCategoryNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetCategoryById(categoryId)).ReturnsAsync((ResponseCategoryDto?)null);

            // Act
            var result = await _controller.GetById(categoryId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetDeleted_ShouldReturnDeletedCategories()
        {
            // Arrange
            var deletedCategories = new List<ResponseCategoryDto> { new(Guid.NewGuid(), "Categoria Deletada") };
            _mockRepository.Setup(r => r.GetDeletedCategories()).ReturnsAsync(deletedCategories);

            // Act
            var result = await _controller.GetDeleted();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetDeletedCategories(), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnOk()
        {
            // Arrange
            var categoryDto = new RequestCategoryDto("Nova Categoria");
            _mockRepository.Setup(r => r.CreateCategory(categoryDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(categoryDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.CreateCategory(categoryDto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenArgumentException_ShouldReturnBadRequest()
        {
            // Arrange
            var categoryDto = new RequestCategoryDto("Nova Categoria");
            _mockRepository.Setup(r => r.CreateCategory(categoryDto))
                .ThrowsAsync(new ArgumentException("Nome inválido"));

            // Act
            var result = await _controller.Create(categoryDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_WhenException_ShouldReturnInternalServerError()
        {
            // Arrange
            var categoryDto = new RequestCategoryDto("Nova Categoria");
            _mockRepository.Setup(r => r.CreateCategory(categoryDto))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.Create(categoryDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var categoryDto = new RequestCategoryDto("Categoria Atualizada");
            _mockRepository.Setup(r => r.UpdateCategory(categoryId, categoryDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(categoryId, categoryDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateCategory(categoryId, categoryDto), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeleteCategory(categoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(categoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.DeleteCategory(categoryId), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RestoreCategory(categoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(categoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.RestoreCategory(categoryId), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.AddProductToCategory(categoryId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.AddProductToCategory(categoryId, productId), Times.Once);
        }

        [Fact]
        public async Task AddProduct_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.AddProductToCategory(categoryId, productId))
                .ThrowsAsync(new KeyNotFoundException("Categoria ou produto não encontrado"));

            // Act
            var result = await _controller.AddProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task RemoveProduct_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RemoveProductFromCategory(categoryId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.RemoveProductFromCategory(categoryId, productId), Times.Once);
        }

        [Fact]
        public async Task RemoveProduct_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RemoveProductFromCategory(categoryId, productId))
                .ThrowsAsync(new KeyNotFoundException("Categoria ou produto não encontrado"));

            // Act
            var result = await _controller.RemoveProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
