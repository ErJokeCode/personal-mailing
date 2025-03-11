using Core.Routes.Admins.Commands;
using FluentValidation;
using FluentValidation.Validators;

namespace Core.Routes.Admins.Validators;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithName("Почта");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithName("Пароль");
    }
}