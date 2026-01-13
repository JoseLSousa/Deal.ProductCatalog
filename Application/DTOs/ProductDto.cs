namespace Application.DTOs
{
    public record ProductDto(
        string Name,
        string Description,
        decimal Price,
        bool Active,
        Guid CategoryId
        );
}
