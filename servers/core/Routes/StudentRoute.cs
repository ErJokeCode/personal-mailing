using Core.Handlers;
using Core.Identity;
using Microsoft.AspNetCore.Builder;

namespace Core.Routes;

public static class StudentRoute
{
    public static void MapStudentRoutes(this WebApplication app)
    {
        var group = app.MapGroup("/core/student");

        group.MapGet("/", StudentHandler.GetAllStudents).RequireAuthorization(Permissions.ViewPolicy);
        group.MapPost("/auth", StudentHandler.AuthStudent).RequireAuthorization(Permissions.CreateAdminsPolicy);

        group.MapPut("/addOnboard/{id}", StudentHandler.AddOnboardStatus)
            .RequireAuthorization(Permissions.CreateAdminsPolicy);

        group.MapGet("/{id}", StudentHandler.GetStudent).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/{id}/chats", ChatHandler.GetStudentChats).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/{id}/courses", StudentHandler.GetStudentCourses).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/{id}/notifications", NotificationHandler.GetStudentNotifications)
            .RequireAuthorization(Permissions.ViewPolicy);
    }
}
