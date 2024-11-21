using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class TemplateRoute
{
    public static void MapTemplateRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/template");

        MapGet(group);
        MapPost(group);
        MapPut(group);
        MapDelete(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", TemplateHandler.GetAllTemplates);
        getGroup.MapGet("/{id}", TemplateHandler.GetTemplate);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/", TemplateHandler.SaveNotificationTemplate)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/{id}", TemplateHandler.UpdateTemplate)
            .AddPermission(Permissions.SendNotifications)
            .DisableAntiforgery();
    }

    public static void MapDelete(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", TemplateHandler.DeleteTemplate).AddPermission(Permissions.SendNotifications);
    }
}
