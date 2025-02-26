using System.Threading.Tasks;
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

        app.Run();
    }
}
