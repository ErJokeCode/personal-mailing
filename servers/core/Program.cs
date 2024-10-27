using System.Threading.Tasks;
using Core.Models;
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

        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        MessageHandlers.Hub = app.Services.GetService<IHubContext<SignalHub>>();

        var group = app.MapGroup("/core").RequireAuthorization("AdminPolicy");

        group.MapPost("/auth", Handlers.HandleAuth);
        group.MapGet("/{id}/courses", Handlers.HandleCourses);
        group.MapGet("/students", Handlers.HandleStudents);
        group.MapPost("/send_notification", Handlers.SendNotification);

        app.Run();
    }
}
