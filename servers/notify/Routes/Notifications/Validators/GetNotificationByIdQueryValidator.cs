using Core.Routes.Notifications.Queries;
using FluentValidation;

namespace Core.Routes.Notifications.Validators;

public class GetNotificationByIdQueryValidator : AbstractValidator<GetNotificationByIdQuery>
{
    public GetNotificationByIdQueryValidator()
    {
        RuleFor(x => x.NotificationId)
            .NotEmpty()
            .WithName("Рассылка");
    }
}