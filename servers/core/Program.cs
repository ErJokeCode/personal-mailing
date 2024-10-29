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

        group.MapPost("/auth", AuthHandler.AuthStudent);
        group.MapGet("/{id}/courses", StudentHandler.GetStudentCourses);
        group.MapGet("/students", StudentHandler.GetStudents);
        group.MapPost("/send_notification", NotificationHandler.SendNotification).DisableAntiforgery();
        group.MapGet("/student_notifications", NotificationHandler.GetStudentNotifications);
        group.MapGet("/admin_notifications", NotificationHandler.GetAdminNotifications);
        group.MapGet("/document", DocumentHandler.GetDocument);

        app.Run();
    }
}
