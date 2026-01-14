using Domain.Entities;
using FluentAssertions;

namespace Tests.Domain
{
    public class TagTests
    {
        [Fact]
        public void Tag_ShouldCreateWithValidName()
        {
            // Arrange & Act
            var tag = new Tag("Tag Teste");

            // Assert
            tag.Name.Should().Be("Tag Teste");
            tag.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            tag.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var tag = new Tag("Tag");
            var oldUpdatedAt = tag.UpdatedAt;

            // Act
            tag.UpdateName("Nova Tag");

            // Assert
            tag.Name.Should().Be("Nova Tag");
            tag.UpdatedAt.Should().BeAfter(oldUpdatedAt);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void UpdateName_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var tag = new Tag("Tag");

            // Act
            Action act = () => tag.UpdateName(invalidName);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O nome da tag não pode ser vazio.*");
        }

        [Fact]
        public void AssignToProduct_WithValidProductId_ShouldUpdateProductId()
        {
            // Arrange
            var tag = new Tag("Tag");
            var productId = Guid.NewGuid();

            // Act
            tag.AssignToProduct(productId);

            // Assert
            tag.ProductId.Should().Be(productId);
        }

        [Fact]
        public void AssignToProduct_WithEmptyGuid_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag");

            // Act
            Action act = () => tag.AssignToProduct(Guid.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("O ID do produto não pode ser vazio.*");
        }

        [Fact]
        public void Delete_ShouldSetDeletedAtAndIsDeleted()
        {
            // Arrange
            var tag = new Tag("Tag");

            // Act
            tag.Delete();

            // Assert
            tag.IsDeleted.Should().BeTrue();
            tag.DeletedAt.Should().NotBeNull();
            tag.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Delete_WhenAlreadyDeleted_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag");
            tag.Delete();

            // Act
            Action act = () => tag.Delete();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A tag já foi deletada.");
        }

        [Fact]
        public void Restore_WhenDeleted_ShouldClearDeletedAt()
        {
            // Arrange
            var tag = new Tag("Tag");
            tag.Delete();

            // Act
            tag.Restore();

            // Assert
            tag.IsDeleted.Should().BeFalse();
            tag.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Restore_WhenNotDeleted_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag");

            // Act
            Action act = () => tag.Restore();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("A tag não está deletada.");
        }

        [Fact]
        public void Operations_OnDeletedTag_ShouldThrowException()
        {
            // Arrange
            var tag = new Tag("Tag");
            tag.Delete();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => tag.UpdateName("Novo Nome"));
            Assert.Throws<InvalidOperationException>(() => tag.AssignToProduct(Guid.NewGuid()));
        }
    }
}
