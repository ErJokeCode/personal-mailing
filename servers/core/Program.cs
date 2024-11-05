using System.Threading.Tasks;
using Core.Messages;
using Core.Routes;
using Core.Utility;
using Microsoft.AspNetCore.Builder;
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

        app.MapStudentRoutes();
        app.MapAdminRoutes();
        app.MapChatRoutes();
        app.MapNotificationRoutes();
        app.MapDocumentRoutes();

        app.Run();
    }
}
