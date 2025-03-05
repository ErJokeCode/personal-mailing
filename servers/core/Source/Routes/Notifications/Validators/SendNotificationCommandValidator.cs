using Core.Routes.Notifications.Commands;
using FluentValidation;

namespace Core.Routes.Notifications.Validators;

// TODO cant send notification or message if content is empty, but theres files attached
// so you cant send just files, change that

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.StudentIds).NotEmpty();
    }
}