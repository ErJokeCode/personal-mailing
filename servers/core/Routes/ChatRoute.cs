using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class ChatRoute
{
    public static void MapChatRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/chat");

        MapGet(group);
        MapPost(group);
        MapPut(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/adminWith/{studentId}", ChatHandler.GetAdminChatWithStudent);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/adminSend", ChatHandler.AdminSendToStudent)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();

        group.MapPost("/studentSend", ChatHandler.StudentSendToAdmin)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/message/{id}/setStatus/{code}", ChatHandler.SetMessageStatus)
            .AddPermission(Permissions.SendNotifications);
    }
}
