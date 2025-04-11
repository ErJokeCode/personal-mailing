using System;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Notify.Data;
using Notify.Models;
using Shared.Context.Notifications;
using Shared.Infrastructure.Errors;

public static class ReadNotificationCommand
{
    public class Request
    {
        public required int NotificationId { get; set; }
        public required Guid StudentId { get; set; }
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

    public static async Task<Results<Ok, BadRequest<ProblemDetails>, ValidationProblem>> Handle(Request request, IValidator<Request> validator, AppDbContext db)
    {
        if (validator.TryValidate(request, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var notification = await db.Notifications.FindAsync(request.NotificationId);

        if (notification is null)
        {
            return Result.Fail(NotificationErrors.NotFound(request.NotificationId)).ToBadRequestProblem();
        }

        var status = notification.Statuses.SingleOrDefault(s => s.StudentId == request.StudentId);

        if (status is null)
        {
            return Result.Fail(NotificationErrors.NotFound(request.NotificationId)).ToBadRequestProblem();
        }

        status.State = NotificationState.Read;
        status.Message = "Прочитано";

        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }
}