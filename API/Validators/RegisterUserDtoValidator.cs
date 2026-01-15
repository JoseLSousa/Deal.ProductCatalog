using Application.DTOs.Auth;
using FluentValidation;

namespace API.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("Email inválido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
                .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
                .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
                .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("A role é obrigatória.")
                .Must(role => role == "Admin" || role == "Editor" || role == "Viewer")
                .WithMessage("Role inválida. Use: Admin, Editor ou Viewer.");
        }
    }
}
