using Core.Routes.Notifications.Commands;
using FluentValidation;

namespace Core.Routes.Notifications.Validators;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.StudentIds).NotEmpty();
    }
}