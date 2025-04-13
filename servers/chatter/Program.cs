using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Shared.Infrastructure.Extensions;

namespace Chatter;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.InitialzieServices();
        app.MapRoutes(typeof(Program).Assembly);

        app.Run();
    }
}
