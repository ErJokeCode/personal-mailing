using Core.Routes.Students.Commands;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class AuthStudentCommandValidator : AbstractValidator<AuthStudentCommand>
{
    public AuthStudentCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.PersonalNumber).NotEmpty();
        RuleFor(x => x.ChatId).NotEmpty();
    }
}