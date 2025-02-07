using Core.Routes.Admins.Commands;
using FluentValidation;
using FluentValidation.Validators;

namespace Core.Routes.Admins.Validators;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MinimumLength(1);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(1);
    }
}