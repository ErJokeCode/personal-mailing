using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Core;

// TODO, put filter options on all methods, that get list of resources

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();

        await app.InitialzieServices();
        app.MapRoutes();

        app.MapGet("/ping", () => TypedResults.Ok())
            .WithDescription("Used for docker container setup");

        app.Run();
    }
}
