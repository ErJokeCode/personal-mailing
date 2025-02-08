using Core.Routes.Students.Commands;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class AuthStudentCommandValidator : AbstractValidator<AuthStudentCommand>
{
    public AuthStudentCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MinimumLength(1);
        RuleFor(x => x.PersonalNumber).NotEmpty().MinimumLength(1);
        RuleFor(x => x.ChatId).NotEmpty().MinimumLength(1);
    }
}