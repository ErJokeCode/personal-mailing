using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class ScheduleRoute
{
    public static void MapScheduleRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/schedule");

        MapGet(group);
        MapPost(group);
        MapPut(group);
        MapDelete(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", ScheduleHandler.GetAllSchedules);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/", ScheduleHandler.AddSchedule).AddPermission(Permissions.SendNotifications);
    }

    public static void MapPut(RouteGroupBuilder group)
    {
    }

    public static void MapDelete(RouteGroupBuilder group)
    {
        group.MapDelete("/{id}", ScheduleHandler.DeleteSchedule).AddPermission(Permissions.SendNotifications);
    }
}
