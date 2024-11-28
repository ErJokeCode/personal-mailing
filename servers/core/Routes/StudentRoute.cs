using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Core.Routes;

public static class StudentRoute
{
    public static void MapStudentRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/student");

        MapGet(group);
        MapPost(group);
        MapPut(group);
    }

    public static void MapGet(RouteGroupBuilder group)
    {
        var getGroup = group.MapGroup("/").AddPermission(Permissions.View);

        getGroup.MapGet("/", StudentHandler.GetAllStudents);
        getGroup.MapGet("/{id}", StudentHandler.GetStudent);
        getGroup.MapGet("/{id}/courses", StudentHandler.GetStudentCourses);

        getGroup.MapGet("/{id}/chats", StudentHandler.GetStudentChats);
        getGroup.MapGet("/{id}/notifications", StudentHandler.GetStudentNotifications);
    }

    public static void MapPost(RouteGroupBuilder group)
    {
        group.MapPost("/auth", StudentHandler.AuthStudent).AddPermission(Permissions.CreateAdmins);
    }

    public static void MapPut(RouteGroupBuilder group)
    {
        group.MapPut("/{id}/onboard/", StudentHandler.AddOnboardStatus).AddPermission(Permissions.ManipulateStudents);

        group.MapPut("/{id}/admin/", StudentHandler.ChangeAdmin).AddPermission(Permissions.CreateAdmins);
    }
}
