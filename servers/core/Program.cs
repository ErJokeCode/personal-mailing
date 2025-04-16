using System.Threading.Tasks;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Shared.Infrastructure.Extensions;

namespace Core;

// TODO add a default sorting method to all methods returning collections

public class Program
{
    public static async Task Main(string[] args)
    {
        DotEnv.Load();

        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();
        app.MapRoutes(typeof(Program).Assembly);

        app.Run();
    }
}
