using API.Controllers;
using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _controller = new ProductController(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithProducts()
        {
            // Arrange
            var products = new List<ResponseProductDto>
            {
                new(Guid.NewGuid(), "Produto 1", "Descrição 1", 10.00m, true, Guid.NewGuid()),
                new(Guid.NewGuid(), "Produto 2", "Descrição 2", 20.00m, true, Guid.NewGuid())
            };
            _mockRepository.Setup(r => r.ListAllProducts(false)).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(products);
            _mockRepository.Verify(r => r.ListAllProducts(false), Times.Once);
        }

        [Fact]
        public async Task GetAll_WithIncludeDeleted_ShouldCallRepositoryWithTrueParameter()
        {
            // Arrange
            _mockRepository.Setup(r => r.ListAllProducts(true)).ReturnsAsync(new List<ResponseProductDto>());

            // Act
            await _controller.GetAll(includeDeleted: true);

            // Assert
            _mockRepository.Verify(r => r.ListAllProducts(true), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenProductExists_ShouldReturnOkWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ResponseProductDto(productId, "Produto", "Descrição", 10.00m, true, Guid.NewGuid());
            _mockRepository.Setup(r => r.GetProductById(productId)).ReturnsAsync(product);

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
            _mockRepository.Setup(r => r.GetProductById(productId)).ReturnsAsync((ResponseProductDto?)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // TESTES OBSOLETOS - Endpoints removidos do controller
        /*
        [Fact]
        public async Task GetByCategory_ShouldReturnProductsInCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var products = new List<ResponseProductDto>
            {
                new(Guid.NewGuid(), "Produto 1", "Descrição", 10.00m, true, categoryId)
            };
            _mockRepository.Setup(r => r.GetProductsByCategory(categoryId)).ReturnsAsync(products);

            // Act
            var result = await _controller.GetByCategory(categoryId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetDeleted_ShouldReturnDeletedProducts()
        {
            // Arrange
            var deletedProducts = new List<ResponseProductDto>
            {
                new(Guid.NewGuid(), "Produto Deletado", "Descrição", 10.00m, false, Guid.NewGuid())
            };
            _mockRepository.Setup(r => r.GetDeletedProducts()).ReturnsAsync(deletedProducts);

            // Act
            var result = await _controller.GetDeleted();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetDeletedProducts(), Times.Once);
        }

        [Fact]
        public async Task GetActive_ShouldReturnActiveProducts()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetActiveProducts()).ReturnsAsync(new List<ResponseProductDto>());

            // Act
            var result = await _controller.GetActive();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetActiveProducts(), Times.Once);
        }

        [Fact]
        public async Task GetInactive_ShouldReturnInactiveProducts()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetInactiveProducts()).ReturnsAsync(new List<ResponseProductDto>());

            // Act
            var result = await _controller.GetInactive();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockRepository.Verify(r => r.GetInactiveProducts(), Times.Once);
        }
        */

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var productDto = new RequestProductDto("Produto", "Descrição", 10.00m, true, Guid.NewGuid());
            _mockRepository.Setup(r => r.CreateProduct(productDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(productDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(201);
            _mockRepository.Verify(r => r.CreateProduct(productDto), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var productDto = new RequestProductDto("Produto Atualizado", "Nova Descrição", 15.00m, true, Guid.NewGuid());
            _mockRepository.Setup(r => r.UpdateProduct(productId, productDto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(productId, productDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateProduct(productId, productDto), Times.Once);
        }

        [Fact]
        public async Task UpdatePrice_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newPrice = 25.00m;
            _mockRepository.Setup(r => r.UpdateProductPrice(productId, newPrice)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePrice(productId, newPrice);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateProductPrice(productId, newPrice), Times.Once);
        }

        [Fact]
        public async Task UpdateDescription_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newDescription = "Nova descrição";
            _mockRepository.Setup(r => r.UpdateProductDescription(productId, newDescription)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDescription(productId, newDescription);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.UpdateProductDescription(productId, newDescription), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeleteProduct(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.DeleteProduct(productId), Times.Once);
        }

        [Fact]
        public async Task Restore_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RestoreProduct(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Restore(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.RestoreProduct(productId), Times.Once);
        }

        [Fact]
        public async Task Activate_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ActivateProduct(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Activate(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.ActivateProduct(productId), Times.Once);
        }

        [Fact]
        public async Task Deactivate_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.DeactivateProduct(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Deactivate(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.DeactivateProduct(productId), Times.Once);
        }

        [Fact]
        public async Task ChangeCategory_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var newCategoryId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ChangeProductCategory(productId, newCategoryId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeCategory(productId, newCategoryId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.ChangeProductCategory(productId, newCategoryId), Times.Once);
        }

        [Fact]
        public async Task AddTag_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tagId = Guid.NewGuid();
            _mockRepository.Setup(r => r.AddTagToProduct(productId, tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddTag(productId, tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.AddTagToProduct(productId, tagId), Times.Once);
        }

        [Fact]
        public async Task RemoveTag_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var tagId = Guid.NewGuid();
            _mockRepository.Setup(r => r.RemoveTagFromProduct(productId, tagId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveTag(productId, tagId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.RemoveTagFromProduct(productId, tagId), Times.Once);
        }

        [Fact]
        public async Task ClearTags_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockRepository.Setup(r => r.ClearProductTags(productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ClearTags(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockRepository.Verify(r => r.ClearProductTags(productId), Times.Once);
        }
    }
}
