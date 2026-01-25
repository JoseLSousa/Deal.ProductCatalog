using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductApplicationService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductApplicationService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new("Produto 1", "Descrição 1", 10.00m, true, Guid.NewGuid()) { ProductId = Guid.NewGuid() },
                new("Produto 2", "Descrição 2", 20.00m, true, Guid.NewGuid()) { ProductId = Guid.NewGuid() }
            };
            _mockService.Setup(s => s.GetActiveProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(products);
            _mockService.Verify(s => s.GetActiveProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenProductExists_ShouldReturnOkWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product("Produto", "Descrição", 10.00m, true, Guid.NewGuid()) { ProductId = productId };
            _mockService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetById_WhenProductNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var productDto = new RequestProductDto("Produto", "Descrição", 10.00m, true, Guid.NewGuid());
            var newId = Guid.NewGuid();
            _mockService.Setup(s => s.CreateProductAsync(productDto.Name, productDto.Description, productDto.Price, productDto.Active, productDto.CategoryId))
                        .ReturnsAsync(newId);

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            result.Should().BeOfType<CreatedAtRouteResult>();
            var created = result as CreatedAtRouteResult;
            created!.RouteName.Should().Be(nameof(ProductController.GetById));
            created.RouteValues!["id"].Should().Be(newId);
            _mockService.Verify(s => s.CreateProductAsync(productDto.Name, productDto.Description, productDto.Price, productDto.Active, productDto.CategoryId), Times.Once);
        }

        [Fact]
        public async Task UpdateName_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newName = "Produto Atualizado";
            _mockService.Setup(s => s.UpdateProductNameAsync(productId, newName)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateName(productId, newName);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.UpdateProductNameAsync(productId, newName), Times.Once);
        }

        [Fact]
        public async Task UpdatePrice_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newPrice = 25.00m;
            _mockService.Setup(s => s.UpdateProductPriceAsync(productId, newPrice)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePrice(productId, newPrice);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.UpdateProductPriceAsync(productId, newPrice), Times.Once);
        }

        [Fact]
        public async Task UpdateDescription_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newDescription = "Nova descrição";
            _mockService.Setup(s => s.UpdateProductDescriptionAsync(productId, newDescription)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDescription(productId, newDescription);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.UpdateProductDescriptionAsync(productId, newDescription), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.DeleteProductAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.RestoreProductAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.RestoreProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task Activate_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.ActivateProductAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Activate(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.ActivateProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task Deactivate_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.DeactivateProductAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deactivate(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.DeactivateProductAsync(productId), Times.Once);
        }

        [Fact]
        public async Task ChangeCategory_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newCategoryId = Guid.NewGuid();
            _mockService.Setup(s => s.ChangeCategoryAsync(productId, newCategoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeCategory(productId, newCategoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.ChangeCategoryAsync(productId, newCategoryId), Times.Once);
        }

        [Fact]
        public async Task AddTag_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.AddTagToProductAsync(productId, tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddTag(productId, tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.AddTagToProductAsync(productId, tagId), Times.Once);
        }

        [Fact]
        public async Task RemoveTag_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tagId = Guid.NewGuid();
            _mockService.Setup(s => s.RemoveTagFromProductAsync(productId, tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveTag(productId, tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.RemoveTagFromProductAsync(productId, tagId), Times.Once);
        }

        [Fact]
        public async Task ClearTags_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockService.Setup(s => s.ClearProductTagsAsync(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ClearTags(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.ClearProductTagsAsync(productId), Times.Once);
        }
    }
}
