using Application.DTOs.Search;
using FluentValidation;

namespace API.Validators
{
    public class ProductSearchDtoValidator : AbstractValidator<ProductSearchDto>
    {
        public ProductSearchDtoValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("A página deve ser maior que zero.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("O tamanho da página deve ser maior que zero.")
                .LessThanOrEqualTo(100).WithMessage("O tamanho da página não pode ser maior que 100.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0).WithMessage("O preço mínimo não pode ser negativo.")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0).WithMessage("O preço máximo não pode ser negativo.")
                .When(x => x.MaxPrice.HasValue);

            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
                .WithMessage("O preço mínimo não pode ser maior que o preço máximo.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => sortBy == null || sortBy == "name" || sortBy == "price" || sortBy == "date")
                .WithMessage("SortBy deve ser 'name', 'price' ou 'date'.")
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy));
        }
    }
}
