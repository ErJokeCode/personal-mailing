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
        MapDelete(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", NotificationHandler.GetAllNotifications);
        getGroup.MapGet("/template", NotificationHandler.GetAllTemplates);

        getGroup.MapGet("/{id}", NotificationHandler.GetNotification);
        getGroup.MapGet("/template/{id}", NotificationHandler.GetTemplate);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/", NotificationHandler.SendNotification)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();

        group.MapPost("/template", NotificationHandler.SaveNotificationTemplate)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/template/{id}", NotificationHandler.UpdateTemplate)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();

        group.MapPut("/{id}/setStatus", NotificationHandler.SetNotificationStatus)
            .AddPermission(Permissions.SendNotifications);
    }

    public static void MapDelete(RouteGroupBuilder group)
    {
        group.MapDelete("/template/{id}", NotificationHandler.DeleteTemplate)
            .AddPermission(Permissions.SendNotifications);
    }
}
