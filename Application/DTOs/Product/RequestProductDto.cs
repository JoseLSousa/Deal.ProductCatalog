using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Product
{
    public record RequestProductDto(
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(200, ErrorMessage = "O nome do produto deve ter no máximo 200 caracteres.")]
        string Name,

        [Required(ErrorMessage = "A descrição do produto é obrigatória.")]
        [StringLength(1000, ErrorMessage = "A descrição do produto deve ter no máximo 1000 caracteres.")]
        string Description,

        [Range(0.01, 999999.99, ErrorMessage = "O preço do produto deve estar entre 0,01 e 99.999,99.")]
        decimal Price,
        bool Active,

        [Required(ErrorMessage = "A categoria do produto é obrigatória.")]
        Guid CategoryId
        );
}
