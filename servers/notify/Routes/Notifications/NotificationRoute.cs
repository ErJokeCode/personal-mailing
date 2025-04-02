using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Notify.Infrastructure.Errors;
using Notify.Infrastructure.Rest;
using Notify.Routes;
using Notify.Routes.Notifications.Commands;
using Notify.Routes.Notifications.DTOs;
using Notify.Routes.Notifications.Maps;
using Notify.Routes.Notifications.Queries;

namespace Notify.Routes.Notifications;

class NotificationRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/notifications");

        group.MapGet("/", GetAllNotifications)
            .WithDescription("Получает все рассылки");

        group.MapPost("/", SendNotification)
            .WithDescription("Отправляет рассылку")
            .DisableAntiforgery();

        group.MapGet("/{notificationId}", GetNotificationById)
            .WithDescription("Получает рассылку по айди");
    }

    public async Task<Results<Ok<NotificationDto>, NotFound<ProblemDetails>, ValidationProblem>> GetNotificationById(
        int notificationId, IValidator<GetNotificationByIdQuery> validator, IMediator mediator
    )
    {
        var query = new GetNotificationByIdQuery()
        {
            NotificationId = notificationId,
        };

        if (validator.TryValidate(query, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await mediator.Send(query);

        if (result.IsFailed)
        {
            return result.ToNotFoundProblem();
        }

        return TypedResults.Ok(result.Value);
    }

    public async Task<PagedList<NotificationDto>> GetAllNotifications([AsParameters] GetAllNotificationsQuery query, IMediator mediator)
    {
        var result = await mediator.Send(query);

        return result;
    }

    public async Task<Results<Ok<NotificationDto>, BadRequest<ProblemDetails>, ValidationProblem>> SendNotification(
        [FromForm] IFormFileCollection documents, [FromForm] string body, IValidator<SendNotificationCommand> validator, IMediator mediator
    )
    {
        var sendDto = new NotificationMapper().Map(body);

        var command = new SendNotificationCommand()
        {
            Content = sendDto?.Content ?? "",
            StudentIds = sendDto?.StudentIds ?? [],
            FormFiles = documents,
        };

        if (validator.TryValidate(command, out var validation))
        {
            return validation.ToValidationProblem();
        }

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            return result.ToBadRequestProblem();
        }

        return TypedResults.Ok(result.Value);
    }
}