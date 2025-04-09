using System.Linq;
using Chatter.Routes.Chats.Commands;
using FluentValidation;

namespace Chatter.Routes.Chats.Validators;

public class SendMessageFromStudentCommandValidator : AbstractValidator<SendMessageFromStudentCommand>
{
    public SendMessageFromStudentCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().When(x => !x.FormFiles?.Any() ?? true);
    }
}