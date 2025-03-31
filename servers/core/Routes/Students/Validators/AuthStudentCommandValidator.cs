using Core.Routes.Students.Commands;
using FluentValidation;

namespace Core.Routes.Admins.Validators;

public class AuthStudentCommandValidator : AbstractValidator<AuthStudentCommand>
{
    public AuthStudentCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithName("Почта");

        RuleFor(x => x.PersonalNumber)
            .NotEmpty()
            .WithName("Номер");

        RuleFor(x => x.ChatId)
            .NotEmpty()
            .WithName("Чат");
    }
}