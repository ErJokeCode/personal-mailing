using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public static class NotificationRoute
{
    public static void MapNotificationRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/notification");

        group.MapPost("/", NotificationHandler.SendNotification)
            .RequireAuthorization(Permissions.SendNotificationsPolicy)
            .DisableAntiforgery();


        group.MapGet("/", NotificationHandler.GetAllNotifications).RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/{id}", NotificationHandler.GetNotification).RequireAuthorization(Permissions.ViewPolicy);

        group.MapPut("/{id}/set-status", NotificationHandler.SetNotificationStatus)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);
    }
}
