using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Notify.Features.Notifications.Commands;
using Notify.Features.Notifications.Queries;
using Shared.Infrastructure.Extensions;

namespace Notify.Features.Notifications;

class NotificationRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/notify/notifications")
            .WithTags("Рассылки");

        group.MapGet("/", GetAllNotificationsQuery.Handle)
            .WithDescription("Получает все рассылки");

        group.MapPost("/", SendNotificationCommand.Handle)
            .WithDescription("Отправляет рассылку")
            .DisableAntiforgery();

        group.MapPost("/read", ReadNotificationCommand.Handle)
            .WithDescription("Делает рассылку прочитанной студентом");

        group.MapGet("/{notificationId}", GetNotificationByIdQuery.Handle)
            .WithDescription("Получает рассылку по айди");
    }
}