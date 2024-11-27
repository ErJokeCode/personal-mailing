using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class NotificationRoute
{
    public static void MapNotificationRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/notification");

        MapGet(group);
        MapPost(group);
        MapPut(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", NotificationHandler.GetAllNotifications);

        getGroup.MapGet("/{id}", NotificationHandler.GetNotification);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/", NotificationHandler.SendNotification)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/{id}/setStatus", NotificationHandler.SetNotificationStatus)
            .AddPermission(Permissions.ManipulateStudents);
    }
}
