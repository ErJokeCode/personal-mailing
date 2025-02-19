using Core.Routes.Chats.Commands;
using FluentValidation;

namespace Core.Routes.Chats.Validators;

public class SendMessageFromStudentCommandValidator : AbstractValidator<SendMessageFromStudentCommand>
{
    public SendMessageFromStudentCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
    }
}