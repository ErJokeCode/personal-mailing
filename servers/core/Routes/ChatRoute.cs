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
        var getGroup = group.MapGroup("/").RequireAuthorization(Permissions.ViewPolicy);

        getGroup.MapGet("/adminWith/{studentId}", ChatHandler.GetAdminChatWithStudent)
            .RequireAuthorization(Permissions.ViewPolicy);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/adminSend", ChatHandler.AdminSendToStudent)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        group.MapPost("/studentSend", ChatHandler.StudentSendToAdmin)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/message/{id}/setStatus/{code}", ChatHandler.SetMessageStatus)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);
    }
}
