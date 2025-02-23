using System.Threading.Tasks;
using Core.Infrastructure.Metadata;
using Microsoft.AspNetCore.Builder;

namespace Core;

// TODO add optional pagination to all methods returning collections

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();

        await app.InitialzieServices();
        app.MapRoutes();

        var routes = app.DiscoverRoutes();
        await app.SyncPermissions(routes);

        app.Run();
    }
}
