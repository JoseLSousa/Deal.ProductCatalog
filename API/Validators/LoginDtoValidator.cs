using Application.DTOs.Auth;
using FluentValidation;

namespace API.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O Email é obrigatório.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
