using System.Threading.Tasks;
using Core.Handlers;
using Core.Identity;
using Core.Messages;
using Core.Models;
using Core.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Startup.LoadEnv(".env");
        Startup.CreateFolder("Documents");

        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        AuthConsumer.Hub = app.Services.GetService<IHubContext<SignalHub>>();

        var group = app.MapGroup("/core");

        // TODO refactor this to Routes/ folder

        group.MapPost("/student/auth", StudentHandler.AuthStudent).RequireAuthorization(Permissions.CreateAdminsPolicy);
        group.MapGet("/student/{id}/courses", StudentHandler.GetStudentCourses)
            .RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/student/{id}", StudentHandler.GetStudent).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/student", StudentHandler.GetAllStudents).RequireAuthorization(Permissions.ViewPolicy);
        group.MapPut("/student/addOnboard/{id}", StudentHandler.AddOnboardStatus)
            .RequireAuthorization(Permissions.CreateAdminsPolicy);

        group.MapGet("/admin", AdminHandler.GetAllAdmins).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/admin/me", AdminHandler.GetAdminMe).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/admin/{id}", AdminHandler.GetAdmin).RequireAuthorization(Permissions.ViewPolicy);
        group.MapPost("/admin/create", AdminHandler.AddNewAdmin).RequireAuthorization(Permissions.CreateAdminsPolicy);

        group.MapPost("/notification", NotificationHandler.SendNotification)
            .RequireAuthorization(Permissions.SendNotificationsPolicy)
            .DisableAntiforgery();

        group.MapGet("/notification", NotificationHandler.GetAllNotifications)
            .RequireAuthorization(Permissions.ViewPolicy);

        group.MapPut("/notification/{id}/set-status", NotificationHandler.SetNotificationStatus)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        group.MapGet("/student/{id}/notifications", NotificationHandler.GetStudentNotifications)
            .RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/admin/notifications", NotificationHandler.GetAdminNotifications)
            .RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/document/{id}", DocumentHandler.GetDocument).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/document/{id}/data", DocumentHandler.GetDocumentData)
            .RequireAuthorization(Permissions.ViewPolicy);

        group.MapGet("/chat/{id}", ChatHandler.GetChatById).RequireAuthorization(Permissions.ViewPolicy);
        group.MapGet("/admin/chats", ChatHandler.GetAdminChats).RequireAuthorization(Permissions.ViewPolicy);
        group.MapPost("/chat/admin-to-student", ChatHandler.AdminSendToStudent)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        group.MapGet("/student/{id}/chats", ChatHandler.GetStudentChats).RequireAuthorization(Permissions.ViewPolicy);
        group.MapPost("/chat/student-to-admin", ChatHandler.StudentSendToAdmin)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        group.MapPut("/message/{id}/set-status", ChatHandler.SetMessageStatus)
            .RequireAuthorization(Permissions.SendNotificationsPolicy);

        app.Run();
    }
}
