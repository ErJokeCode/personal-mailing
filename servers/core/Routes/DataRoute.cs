using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class DataRoute
{
    public static void MapDataRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/data");

        MapGet(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/permissions", DataHandler.GetAllPermissions);
        getGroup.MapGet("/templates", DataHandler.GetAllTemplates);
    }
}
