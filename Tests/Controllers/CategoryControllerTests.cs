using API.Controllers;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryApplicationService> _mockService;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockService = new Mock<ICategoryApplicationService>();
            _controller = new CategoryController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithCategories()
        {
            // Arrange
            var categories = new List<Category>
            {
                new("Categoria 1") { CategoryId = Guid.NewGuid() },
                new("Categoria 2") { CategoryId = Guid.NewGuid() }
            };
            _mockService.Setup(s => s.GetActiveCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(categories);
        }

        [Fact]
        public async Task GetWithActiveProducts_ShouldReturnOk()
        {
            // Arrange
            var categories = new List<Category> { new("Categoria") { CategoryId = Guid.NewGuid() } };
            _mockService.Setup(s => s.GetCategoriesWithActiveProductsAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetWithActiveProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.GetCategoriesWithActiveProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenCategoryExists_ShouldReturnOkWithCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Categoria") { CategoryId = categoryId };
            _mockService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetCategoryById(categoryId);

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
            _mockService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category?)null);

            // Act
            var result = await _controller.GetCategoryById(categoryId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetWithProducts_WhenCategoryExists_ShouldReturnOk()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category("Categoria") { CategoryId = categoryId };
            _mockService.Setup(s => s.GetCategoryWithProductsAsync(categoryId)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetWithProducts(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.GetCategoryWithProductsAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task GetByName_WhenCategoryExists_ShouldReturnOk()
        {
            // Arrange
            var category = new Category("Categoria") { CategoryId = Guid.NewGuid() };
            _mockService.Setup(s => s.GetCategoryByNameAsync(category.Name)).ReturnsAsync(category);

            // Act
            var result = await _controller.GetByName(category.Name);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockService.Verify(s => s.GetCategoryByNameAsync(category.Name), Times.Once);
        }

        [Fact]
        public async Task HasActiveProducts_ShouldReturnOk()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockService.Setup(s => s.HasActiveProductsAsync(categoryId)).ReturnsAsync(true);

            // Act
            var result = await _controller.HasActiveProducts(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var ok = result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(new { categoryId, hasActiveProducts = true });
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var name = "Nova Categoria";
            var newId = Guid.NewGuid();
            _mockService.Setup(s => s.CreateCategoryAsync(name)).ReturnsAsync(newId);

            // Act
            var result = await _controller.Create(name);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();
            var created = result as CreatedAtRouteResult;
            created!.RouteName.Should().Be(nameof(CategoryController.GetCategoryById));
            created.RouteValues!["id"].Should().Be(newId);
        }

        [Fact]
        public async Task Create_WhenArgumentException_ShouldReturnBadRequest()
        {
            // Arrange
            var name = "Nova Categoria";
            _mockService.Setup(s => s.CreateCategoryAsync(name))
                .ThrowsAsync(new ArgumentException("Nome inválido"));

            // Act
            var result = await _controller.Create(name);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_WhenException_ShouldReturnInternalServerError()
        {
            // Arrange
            var name = "Nova Categoria";
            _mockService.Setup(s => s.CreateCategoryAsync(name))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _controller.Create(name);

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
            var newName = "Categoria Atualizada";
            _mockService.Setup(s => s.UpdateCategoryNameAsync(categoryId, newName)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateName(categoryId, newName);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.UpdateCategoryNameAsync(categoryId, newName), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteCategoryAsync(categoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(categoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.DeleteCategoryAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _mockService.Setup(s => s.RestoreCategoryAsync(categoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(categoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.RestoreCategoryAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task AddProduct_ShouldReturnNoContent()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.AddProductToCategoryAsync(categoryId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.AddProductToCategoryAsync(categoryId, productId), Times.Once);
        }

        [Fact]
        public async Task AddProduct_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.AddProductToCategoryAsync(categoryId, productId))
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
            _mockService.Setup(s => s.RemoveProductFromCategoryAsync(categoryId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.RemoveProductFromCategoryAsync(categoryId, productId), Times.Once);
        }

        [Fact]
        public async Task RemoveProduct_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.RemoveProductFromCategoryAsync(categoryId, productId))
                .ThrowsAsync(new KeyNotFoundException("Categoria ou produto não encontrado"));

            // Act
            var result = await _controller.RemoveProduct(categoryId, productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
