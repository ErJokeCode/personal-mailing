using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public static class ChatRoute
{
    public static void MapChatRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/chat");

        group.MapGet("/{id}", ChatHandler.GetChatById).RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/admin-with/{studentId}", ChatHandler.GetAdminChatWithStudent)
            .RequireAuthorization(Permissions.ViewPolicy);

        group.MapPost("/admin-to-student", ChatHandler.AdminSendToStudent)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        group.MapPost("/student-to-admin", ChatHandler.StudentSendToAdmin)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        app.MapPut("/core/message/{id}/set-status", ChatHandler.SetMessageStatus)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);
    }
}
