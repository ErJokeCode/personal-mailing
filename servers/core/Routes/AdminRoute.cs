using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class AdminRoute
{
    public static void MapAdminRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/admin");

        MapGet(group);
        MapPost(group);
        MapPut(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", AdminHandler.GetAllAdmins);
        getGroup.MapGet("/me", AdminHandler.GetAdminMe);
        getGroup.MapGet("/byEmail/{email}", AdminHandler.GetAdminByEmail);

        getGroup.MapGet("/chats", AdminHandler.GetAdminChats);
        getGroup.MapGet("/notifications", AdminHandler.GetAdminNotifications);

        getGroup.MapGet("/{id}", AdminHandler.GetAdmin);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/create", AdminHandler.AddAdmin).AddPermission(Permissions.CreateAdmins);
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/{adminId}/permissions", AdminHandler.UpdatePermissions).AddPermission(Permissions.CreateAdmins);
    }
}
