using API.Controllers;
using Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers
{
    public class ReportsControllerTests
    {
        private readonly Mock<IExportService> _mockExportService;
        private readonly Mock<IAuditLogService> _mockAuditService;
        private readonly ReportsController _controller;

        public ReportsControllerTests()
        {
            _mockExportService = new Mock<IExportService>();
            _mockAuditService = new Mock<IAuditLogService>();
            _controller = new ReportsController(_mockExportService.Object, _mockAuditService.Object);

            // Configurar contexto HTTP com usuário autenticado
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task ExportCsv_ShouldReturnFileWithCorrectContentType()
        {
            // Arrange
            var csvBytes = System.Text.Encoding.UTF8.GetBytes("Nome,Descrição,Preço\nProduto 1,Descrição 1,10.00");

            _mockExportService.Setup(s => s.GenerateCsvReportAsync())
                .ReturnsAsync(csvBytes);

            _mockAuditService.Setup(s => s.LogAsync(It.IsAny<Application.DTOs.LogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ExportCsv();

            // Assert
            result.Should().BeOfType<FileContentResult>();
            var fileResult = result as FileContentResult;
            fileResult!.ContentType.Should().Be("text/csv");
            fileResult.FileContents.Should().BeEquivalentTo(csvBytes);
            fileResult.FileDownloadName.Should().Contain("produtos_relatorio_");
            fileResult.FileDownloadName.Should().EndWith(".csv");
        }

        [Fact]
        public async Task ExportJson_ShouldReturnFileWithCorrectContentType()
        {
            // Arrange
            var json = "{\"products\": []}";

            _mockExportService.Setup(s => s.GenerateJsonReportAsync())
                .ReturnsAsync(json);

            _mockAuditService.Setup(s => s.LogAsync(It.IsAny<Application.DTOs.LogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ExportJson();

            // Assert
            result.Should().BeOfType<FileContentResult>();
            var fileResult = result as FileContentResult;
            fileResult!.ContentType.Should().Be("application/json");
            fileResult.FileDownloadName.Should().Contain("produtos_relatorio_");
            fileResult.FileDownloadName.Should().EndWith(".json");
        }

        [Fact]
        public async Task GetReportData_ShouldReturnOkWithReportData()
        {
            // Arrange
            var reportData = new Application.DTOs.Export.ProductReportDto
            {
                Products = new List<Application.DTOs.Export.ProductReportItemDto>
                {
                    new()
                    {
                        Name = "Produto 1",
                        Description = "Descrição 1",
                        Category = "Eletrônicos",
                        Price = 1000.00m,
                        Tags = "notebook,dell",
                        CreatedAt = DateTime.UtcNow
                    }
                },
                Statistics = new Application.DTOs.Export.ReportStatisticsDto
                {
                    TotalActiveProducts = 1,
                    AveragePrice = 1000.00m
                }
            };

            _mockExportService.Setup(s => s.GetReportDataAsync())
                .ReturnsAsync(reportData);

            // Act
            var result = await _controller.GetReportData();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(reportData);
        }

        [Fact]
        public async Task ExportCsv_WhenExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            _mockExportService.Setup(s => s.GenerateCsvReportAsync())
                .ThrowsAsync(new Exception("Erro ao gerar CSV"));

            // Act
            var result = await _controller.ExportCsv();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }
    }
}
