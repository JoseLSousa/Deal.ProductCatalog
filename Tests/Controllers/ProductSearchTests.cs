using API.Controllers;
using Application.DTOs.Search;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class ProductSearchTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly ProductController _controller;

        public ProductSearchTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _controller = new ProductController(_mockRepository.Object);
        }

        [Fact]
        public async Task Search_WithValidParameters_ShouldReturnOkWithResults()
        {
            // Arrange
            var searchDto = new ProductSearchDto
            {
                Term = "notebook",
                MinPrice = 1000,
                MaxPrice = 5000,
                Page = 1,
                PageSize = 10
            };

            var searchResult = new ProductSearchResultDto
            {
                Items = new List<Application.DTOs.ResponseProductDto>
                {
                    new(Guid.NewGuid(), "Notebook Dell", "Descrição", 3500.00m, true, Guid.NewGuid())
                },
                TotalItems = 1,
                TotalPages = 1,
                CurrentPage = 1,
                PageSize = 10,
                AveragePrice = 3500.00m
            };

            _mockRepository.Setup(r => r.SearchProductsAsync(searchDto))
                .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(searchResult);
            _mockRepository.Verify(r => r.SearchProductsAsync(searchDto), Times.Once);
        }

        [Fact]
        public async Task Search_WithEmptyResults_ShouldReturnOkWithEmptyList()
        {
            // Arrange
            var searchDto = new ProductSearchDto
            {
                Term = "produto_inexistente",
                Page = 1,
                PageSize = 10
            };

            var searchResult = new ProductSearchResultDto
            {
                Items = new List<Application.DTOs.ResponseProductDto>(),
                TotalItems = 0,
                TotalPages = 0,
                CurrentPage = 1,
                PageSize = 10,
                AveragePrice = 0
            };

            _mockRepository.Setup(r => r.SearchProductsAsync(searchDto))
                .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultData = okResult!.Value as ProductSearchResultDto;
            resultData!.Items.Should().BeEmpty();
            resultData.TotalItems.Should().Be(0);
        }

        [Fact]
        public async Task Search_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var searchDto = new ProductSearchDto
            {
                Page = 2,
                PageSize = 5
            };

            var searchResult = new ProductSearchResultDto
            {
                Items = new List<Application.DTOs.ResponseProductDto>
                {
                    new(Guid.NewGuid(), "Produto 6", "Descrição", 10.00m, true, Guid.NewGuid()),
                    new(Guid.NewGuid(), "Produto 7", "Descrição", 20.00m, true, Guid.NewGuid())
                },
                TotalItems = 12,
                TotalPages = 3,
                CurrentPage = 2,
                PageSize = 5,
                AveragePrice = 15.00m
            };

            _mockRepository.Setup(r => r.SearchProductsAsync(searchDto))
                .ReturnsAsync(searchResult);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultData = okResult!.Value as ProductSearchResultDto;
            resultData!.CurrentPage.Should().Be(2);
            resultData.TotalPages.Should().Be(3);
        }
    }
}
