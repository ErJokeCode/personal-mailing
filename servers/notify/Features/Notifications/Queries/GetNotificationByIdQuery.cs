using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notify.Data;
using Notify.Features.Notifications.DTOs;
using Notify.Routes.Notifications.Maps;
using Shared.Context.Notifications;
using Shared.Infrastructure.Errors;

namespace Notify.Features.Notifications.Queries;

public static class GetNotificationByIdQuery
{
    public class Request
    {
        public required int NotificationId { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty()
                .WithName("Рассылка");
        }
    }

    public static async Task<Results<Ok<NotificationDto>, NotFound<ProblemDetails>, ValidationProblem>> Handle(
        int notificationId,
        IValidator<Request> validator,
        AppDbContext db,
        NotificationMapper notificationMapper
    )
    {
        var request = new Request()
        {
            NotificationId = notificationId,
        };

        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var notification = await db.Notifications
            .Include(n => n.Admin)
            .Include(n => n.Students)
            .AsSplitQuery()
            .SingleOrDefaultAsync(n => n.Id == request.NotificationId);

        if (notification is null)
        {
            return Result.Fail<NotificationDto>(NotificationErrors.NotFound(request.NotificationId)).ToNotFoundProblem();
        }

        return TypedResults.Ok(notificationMapper.Map(notification));
    }
}