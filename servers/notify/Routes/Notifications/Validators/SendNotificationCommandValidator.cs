using System.Linq;
using FluentValidation;
using Notify.Routes.Notifications.Commands;

namespace Notify.Routes.Notifications.Validators;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .When(x => !x.FormFiles?.Any() ?? true)
            .WithName("Содержание");

        RuleFor(x => x.StudentIds)
            .NotEmpty()
            .WithName("Студенты");
    }
}