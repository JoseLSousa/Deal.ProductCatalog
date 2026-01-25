using API.Controllers;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class ProductSearchTests
    {
        private readonly Mock<IProductApplicationService> _mockService;
        private readonly ProductController _controller;

        public ProductSearchTests()
        {
            _mockService = new Mock<IProductApplicationService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task Search_WithValidParameters_ShouldReturnOkWithResults()
        {
            // Arrange
            const string term = "notebook";
            var searchResult = new List<Product>
            {
                new("Notebook Dell", "Descrição", 3500.00m, true, Guid.NewGuid()) { ProductId = Guid.NewGuid() }
            };

            _mockService.Setup(s => s.SearchProductsByNameAsync(term))
                        .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.SearchByName(term);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(searchResult);
            _mockService.Verify(s => s.SearchProductsByNameAsync(term), Times.Once);
        }

        [Fact]
        public async Task Search_WithEmptyResults_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            const string term = "produto_inexistente";
            var searchResult = new List<Product>();

            _mockService.Setup(s => s.SearchProductsByNameAsync(term))
                        .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.SearchByName(term);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultData = okResult!.Value as IReadOnlyCollection<Product>;
            resultData!.Should().BeEmpty();
        }
    }
}
