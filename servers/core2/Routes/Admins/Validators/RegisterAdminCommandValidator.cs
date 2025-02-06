using Core.Routes.Admins.Commands;
using FluentValidation;
using FluentValidation.Validators;

namespace Core.Routes.Admins.Validators;

public class RegisterAdminCommandValidator : AbstractValidator<RegisterAdminCommand>
{
    public RegisterAdminCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(1);
    }
}