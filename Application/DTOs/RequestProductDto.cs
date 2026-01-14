namespace Application.DTOs
{
    public record RequestProductDto(
        string Name,
        string Description,
        decimal Price,
        bool Active,
        Guid CategoryId
        );
}
