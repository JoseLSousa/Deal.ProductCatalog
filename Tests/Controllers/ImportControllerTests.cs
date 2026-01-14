using API.Controllers;
using Application.DTOs.Import;
using Application.Interfaces;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace Tests.Controllers
{
    public class ImportControllerTests
    {
        private readonly Mock<IImportService> _mockImportService;
        private readonly Mock<IAuditLogService> _mockAuditService;
        private readonly ImportController _controller;

        public ImportControllerTests()
        {
            _mockImportService = new Mock<IImportService>();
            _mockAuditService = new Mock<IAuditLogService>();
            _controller = new ImportController(_mockImportService.Object, _mockAuditService.Object);

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
        public async Task ImportProducts_WithSuccessfulImport_ShouldReturnOkWithResult()
        {
            // Arrange
            var importResult = new ImportResultDto
            {
                TotalFetched = 20,
                Imported = 18,
                Skipped = 2,
                Messages = new List<string>
                {
                    "Produto 1 importado com sucesso.",
                    "Produto 2 já existe."
                }
            };

            _mockImportService.Setup(s => s.ImportFromExternalApiAsync(It.IsAny<string>()))
                .ReturnsAsync(importResult);

            _mockAuditService.Setup(s => s.LogAsync(It.IsAny<Application.DTOs.LogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            
            _mockImportService.Verify(s => s.ImportFromExternalApiAsync(It.IsAny<string>()), Times.Once);
            _mockAuditService.Verify(s => s.LogAsync(It.IsAny<Application.DTOs.LogDto>()), Times.Once);
        }

        [Fact]
        public async Task ImportProducts_WithNoProductsFound_ShouldReturnOkWithEmptyResult()
        {
            // Arrange
            var importResult = new ImportResultDto
            {
                TotalFetched = 0,
                Imported = 0,
                Skipped = 0,
                Messages = new List<string> { "Nenhum produto encontrado na API externa." }
            };

            _mockImportService.Setup(s => s.ImportFromExternalApiAsync(It.IsAny<string>()))
                .ReturnsAsync(importResult);

            _mockAuditService.Setup(s => s.LogAsync(It.IsAny<Application.DTOs.LogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ImportProducts();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task ImportProducts_WhenExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            _mockImportService.Setup(s => s.ImportFromExternalApiAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro ao consumir API externa"));

            // Act
            var result = await _controller.ImportProducts();

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult!.StatusCode.Should().Be(500);
        }
    }
}
