using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

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

        var group = app.MapGroup("/core").RequireAuthorization("AdminPolicy");

        // group.MapPost("/auth", Handlers.HandleAuth);
        // group.MapGet("/{id}/courses", Handlers.HandleCourses);
        // group.MapGet("/students", Handlers.HandleStudents);
        // group.MapPost("/send", Handlers.SendNotification);

        app.Run();
    }
}
