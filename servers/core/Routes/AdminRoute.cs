using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public static class AdminRoute
{
    public static void MapAdminRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/admin");

        group.MapGet("/", AdminHandler.GetAllAdmins).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/me", AdminHandler.GetAdminMe).RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/notifications", NotificationHandler.GetAdminNotifications)
            .RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/chats", ChatHandler.GetAdminChats).RequireAuthorization(Permissions.ViewPolicy);

        group.MapPost("/create", AdminHandler.AddNewAdmin).RequireAuthorization(Permissions.CreateAdminsPolicy);
        group.MapGet("/{id}", AdminHandler.GetAdmin).RequireAuthorization(Permissions.ViewPolicy);
    }
}
