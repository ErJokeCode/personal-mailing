using FluentValidation;
using Notify.Routes.Notifications.Queries;

namespace Notify.Routes.Notifications.Validators;

public class GetNotificationByIdQueryValidator : AbstractValidator<GetNotificationByIdQuery>
{
    public GetNotificationByIdQueryValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithName("Рассылка");
    }
}