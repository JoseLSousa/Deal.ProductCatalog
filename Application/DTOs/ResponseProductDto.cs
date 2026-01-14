namespace Application.DTOs
{
    public record ResponseProductDto(
        Guid ProductId,
        string Name,
        string Description,
        decimal Price,
        bool Active,
        Guid CategoryId
        );
}
