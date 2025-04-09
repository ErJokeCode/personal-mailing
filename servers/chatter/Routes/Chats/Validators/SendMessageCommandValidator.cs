using System.Linq;
using Chatter.Routes.Chats.Commands;
using FluentValidation;

namespace Chatter.Routes.Chats.Validators;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .When(x => !x.FormFiles?.Any() ?? true)
            .WithName("Содержание");
    }
}