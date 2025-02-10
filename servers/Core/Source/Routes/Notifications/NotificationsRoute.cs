using System;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Notifications.Commands;
using Core.Routes.Notifications.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.Routes.Notifications;

class NotificationsRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/notifications");

        group.MapPost("/", SendNotification)
            .WithDescription("Sends a notification");
    }

    public async Task<Results<Ok<NotificationDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendNotification(
        SendNotificationCommand command, IValidator<SendNotificationCommand> validator, IMediator mediator
    )
    {
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToBadRequestProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}