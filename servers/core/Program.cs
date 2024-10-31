using System.Threading.Tasks;
using Core.Handlers;
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

        var group = app.MapGroup("/core").RequireAuthorization("AdminPolicy");

        group.MapPost("/student/auth", StudentHandler.AuthStudent);
        group.MapGet("/student/{id}/courses", StudentHandler.GetStudentCourses);
        group.MapGet("/student", StudentHandler.GetStudents);

        group.MapPost("/admin/create", AdminHandler.AddNewAdmin);
        group.MapGet("/admin", AdminHandler.GetAdmins);

        group.MapPost("/notification", NotificationHandler.SendNotification).DisableAntiforgery();
        group.MapGet("/student/notifications", NotificationHandler.GetStudentNotifications);
        group.MapGet("/admin/notifications", NotificationHandler.GetAdminNotifications);

        group.MapGet("/document", DocumentHandler.GetDocument);

        app.Run();
    }
}
