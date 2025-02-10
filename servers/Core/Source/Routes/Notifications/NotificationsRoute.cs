using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.Errors;
using Core.Routes.Notifications.Commands;
using Core.Routes.Notifications.Dtos;
using Core.Routes.Notifications.Queries;
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

        group.MapGet("/", GetAllNotifications)
            .WithDescription("Gets all notifications");

        group.MapGet("/{notificationId}", GetNotificationById)
            .WithDescription("Gets a notification by id");
    }

    public async Task<Results<Ok<NotificationDto>, NotFound<ProblemDetails>, ValidationProblem>> GetNotificationById(
        int notificationId, IValidator<GetNotificationByIdQuery> validator, IMediator mediator
    )
    {
        var query = new GetNotificationByIdQuery()
        {
            NotificationId = notificationId,
        };

        var validationResult = await validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblem();
        }

        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllNotifications(IMediator mediator)
    {
        var query = new GetAllNotificationsQuery();

        var result = await mediator.Send(query);

        return result;
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