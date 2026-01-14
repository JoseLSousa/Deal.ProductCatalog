using Application.DTOs;
using FluentValidation;

namespace API.Validators
{
    public class RequestProductDtoValidator : AbstractValidator<RequestProductDto>
    {
        public RequestProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .MaximumLength(200).WithMessage("O nome não pode ter mais de 200 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("A descrição é obrigatória.")
                .MaximumLength(1000).WithMessage("A descrição não pode ter mais de 1000 caracteres.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("O preço deve ser maior que zero.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("A categoria é obrigatória.");
        }
    }
}
