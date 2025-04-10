using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Notify.Features.Notifications.Commands;
using Notify.Features.Notifications.Queries;

namespace Notify.Features.Notifications;

class NotificationRoute : IRoute
{
    public void MapRoutes(WebApplication app)
    {
        var group = app.MapGroup("/core/notifications");

        group.MapGet("/", GetAllNotificationsQuery.Handle)
            .WithDescription("Получает все рассылки");

        group.MapPost("/", SendNotificationCommand.Handle)
            .WithDescription("Отправляет рассылку")
            .DisableAntiforgery();

        group.MapGet("/{notificationId}", GetNotificationByIdQuery.Handle)
            .WithDescription("Получает рассылку по айди");
    }
}