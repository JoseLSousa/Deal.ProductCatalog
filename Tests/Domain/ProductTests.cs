using Domain.Entities;
using FluentAssertions;

namespace Tests.Domain
{
    public class ProductTests
    {
        private readonly Guid _categoryId = Guid.NewGuid();

        [Fact]
        public void Product_ShouldCreateWithValidParameters()
        {
            // Arrange & Act
            var product = new Product("Produto Teste", "Descrição", 10.00m, true, _categoryId);

            // Assert
            product.Name.Should().Be("Produto Teste");
            product.Description.Should().Be("Descrição");
            product.Price.Should().Be(10.00m);
            product.Active.Should().BeTrue();
            product.CategoryId.Should().Be(_categoryId);
            product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            product.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateNameAndTimestamp()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            var oldUpdatedAt = product.UpdatedAt;

            // Act
            product.UpdateName("Novo Nome");

            // Assert
            product.Name.Should().Be("Novo Nome");
            product.UpdatedAt.Should().BeAfter(oldUpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateName_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.UpdateName(invalidName);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O nome do produto não pode ser vazio.*");
        }

        [Fact]
        public void UpdateDescription_WithValidDescription_ShouldUpdate()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            product.UpdateDescription("Nova Descrição");

            // Assert
            product.Description.Should().Be("Nova Descrição");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateDescription_WithInvalidDescription_ShouldThrowException(string invalidDescription)
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.UpdateDescription(invalidDescription);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("A descrição do produto não pode ser vazia.*");
        }

        [Fact]
        public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            product.UpdatePrice(20.00m);

            // Assert
            product.Price.Should().Be(20.00m);
        }

        [Fact]
        public void UpdatePrice_WithNegativePrice_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.UpdatePrice(-5.00m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O preço não pode ser negativo.*");
        }

        [Fact]
        public void Activate_ShouldSetActiveToTrue()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, false, _categoryId);

            // Act
            product.Activate();

            // Assert
            product.Active.Should().BeTrue();
        }

        [Fact]
        public void Deactivate_ShouldSetActiveToFalse()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            product.Deactivate();

            // Assert
            product.Active.Should().BeFalse();
        }

        [Fact]
        public void ChangeCategory_WithValidCategoryId_ShouldUpdateCategory()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            var newCategoryId = Guid.NewGuid();

            // Act
            product.ChangeCategory(newCategoryId);

            // Assert
            product.CategoryId.Should().Be(newCategoryId);
        }

        [Fact]
        public void ChangeCategory_WithEmptyGuid_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.ChangeCategory(Guid.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O ID da categoria não pode ser vazio.*");
        }

        [Fact]
        public void AddTag_WithValidTag_ShouldAddToCollection()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            var tag = new Tag("Tag Teste");

            // Act
            product.AddTag(tag);

            // Assert
            product.Tags.Should().Contain(tag);
            product.Tags.Count.Should().Be(1);
        }

        [Fact]
        public void AddTag_WithNullTag_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.AddTag(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddTag_DuplicateTag_ShouldNotAddAgain()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            var tag = new Tag("Tag Teste");
            product.AddTag(tag);

            // Act
            product.AddTag(tag);

            // Assert
            product.Tags.Count.Should().Be(1);
        }

        [Fact]
        public void RemoveTag_WithExistingTag_ShouldRemoveFromCollection()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            var tag = new Tag("Tag Teste");
            product.AddTag(tag);

            // Act
            product.RemoveTag(tag);

            // Assert
            product.Tags.Should().NotContain(tag);
            product.Tags.Count.Should().Be(0);
        }

        [Fact]
        public void RemoveTag_WithNullTag_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.RemoveTag(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ClearTags_ShouldRemoveAllTags()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            product.AddTag(new Tag("Tag 1"));
            product.AddTag(new Tag("Tag 2"));

            // Act
            product.ClearTags();

            // Assert
            product.Tags.Should().BeEmpty();
        }

        [Fact]
        public void Delete_ShouldSetDeletedAtAndIsDeleted()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            product.Delete();

            // Assert
            product.IsDeleted.Should().BeTrue();
            product.DeletedAt.Should().NotBeNull();
            product.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            product.Delete();

            // Act
            Action act = () => product.Delete();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("O produto já foi deletado.");
        }

        [Fact]
        public void Restore_WhenDeleted_ShouldClearDeletedAt()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            product.Delete();

            // Act
            product.Restore();

            // Assert
            product.IsDeleted.Should().BeFalse();
            product.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Restore_WhenNotDeleted_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);

            // Act
            Action act = () => product.Restore();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("O produto não está deletado.");
        }

        [Fact]
        public void Operations_OnDeletedProduct_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Produto", "Descrição", 10.00m, true, _categoryId);
            product.Delete();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateName("Novo Nome"));
            Assert.Throws<InvalidOperationException>(() => product.UpdateDescription("Nova Descrição"));
            Assert.Throws<InvalidOperationException>(() => product.UpdatePrice(20.00m));
            Assert.Throws<InvalidOperationException>(() => product.Activate());
            Assert.Throws<InvalidOperationException>(() => product.Deactivate());
            Assert.Throws<InvalidOperationException>(() => product.ChangeCategory(Guid.NewGuid()));
            Assert.Throws<InvalidOperationException>(() => product.AddTag(new Tag("Tag")));
            Assert.Throws<InvalidOperationException>(() => product.RemoveTag(new Tag("Tag")));
            Assert.Throws<InvalidOperationException>(() => product.ClearTags());
        }
    }
}
