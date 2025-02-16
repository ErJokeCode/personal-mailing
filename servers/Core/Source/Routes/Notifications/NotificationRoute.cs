using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
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

class NotificationRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/notifications")
            .RequireAuthorization();

        group.MapPost("/", SendNotification)
            .WithDescription("Sends a notification")
            .DisableAntiforgery();

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

    public async Task<IEnumerable<NotificationDto>> GetAllNotifications([AsParameters] GetAllNotificationsQuery query, IMediator mediator)
    {
        var result = await mediator.Send(query);

        return result;
    }

    private class NotificationDetails
    {
        public required string Content { get; set; }
        public required IEnumerable<Guid> StudentIds { get; set; }
    }

    public async Task<Results<Ok<NotificationDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendNotification(
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<SendNotificationCommand> validator, IMediator mediator
    )
    {
        var details = JsonSerializer.Deserialize<NotificationDetails>(
            body,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        var command = new SendNotificationCommand()
        {
            Content = details?.Content ?? "",
            StudentIds = details?.StudentIds ?? [],
            FormFiles = documents,
        };

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