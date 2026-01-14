using Domain.Entities;
using FluentAssertions;

namespace Tests.Domain
{
    public class CategoryTests
    {
        [Fact]
        public void Category_ShouldCreateWithValidName()
        {
            // Arrange & Act
            var category = new Category("Categoria Teste");

            // Assert
            category.Name.Should().Be("Categoria Teste");
            category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            category.IsDeleted.Should().BeFalse();
            category.Products.Should().BeEmpty();
        }

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var category = new Category("Categoria");
            var oldUpdatedAt = category.UpdatedAt;

            // Act
            category.UpdateName("Nova Categoria");

            // Assert
            category.Name.Should().Be("Nova Categoria");
            category.UpdatedAt.Should().BeAfter(oldUpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateName_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var category = new Category("Categoria");

            // Act
            Action act = () => category.UpdateName(invalidName);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O nome da categoria não pode ser vazio.*");
        }

        [Fact]
        public void AddProduct_WithValidProduct_ShouldAddToCollection()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);

            // Act
            category.AddProduct(product);

            // Assert
            category.Products.Should().Contain(product);
            category.Products.Count.Should().Be(1);
        }

        [Fact]
        public void AddProduct_WithNullProduct_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");

            // Act
            Action act = () => category.AddProduct(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddProduct_DuplicateProduct_ShouldNotAddAgain()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            category.AddProduct(product);

            // Act
            category.AddProduct(product);

            // Assert
            category.Products.Count.Should().Be(1);
        }

        [Fact]
        public void RemoveProduct_WithExistingProduct_ShouldRemoveFromCollection()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            category.AddProduct(product);

            // Act
            category.RemoveProduct(product);

            // Assert
            category.Products.Should().NotContain(product);
            category.Products.Count.Should().Be(0);
        }

        [Fact]
        public void RemoveProduct_WithNullProduct_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");

            // Act
            Action act = () => category.RemoveProduct(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Delete_WithNoActiveProducts_ShouldSetDeletedAt()
        {
            // Arrange
            var category = new Category("Categoria");

            // Act
            category.Delete();

            // Assert
            category.IsDeleted.Should().BeTrue();
            category.DeletedAt.Should().NotBeNull();
            category.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Delete_WithActiveProducts_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            category.AddProduct(product);

            // Act
            Action act = () => category.Delete();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Não é possível deletar uma categoria com produtos ativos.");
        }

        [Fact]
        public void Delete_WithDeletedProducts_ShouldNotThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);
            product.Delete();
            category.AddProduct(product);

            // Act
            category.Delete();

            // Assert
            category.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            category.Delete();

            // Act
            Action act = () => category.Delete();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A categoria já foi deletada.");
        }

        [Fact]
        public void Restore_WhenDeleted_ShouldClearDeletedAt()
        {
            // Arrange
            var category = new Category("Categoria");
            category.Delete();

            // Act
            category.Restore();

            // Assert
            category.IsDeleted.Should().BeFalse();
            category.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Restore_WhenNotDeleted_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");

            // Act
            Action act = () => category.Restore();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A categoria não está deletada.");
        }

        [Fact]
        public void Operations_OnDeletedCategory_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Categoria");
            category.Delete();
            var product = new Product("Produto", "Descrição", 10.00m, true, category.CategoryId);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => category.UpdateName("Novo Nome"));
            Assert.Throws<InvalidOperationException>(() => category.AddProduct(product));
            Assert.Throws<InvalidOperationException>(() => category.RemoveProduct(product));
        }
    }
}
