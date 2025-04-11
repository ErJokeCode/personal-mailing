using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Shared.Infrastructure.Extensions;

namespace Notify;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.InitializeServices();
        app.MapRoutes(typeof(Program).Assembly);

        app.Run();
    }
}
