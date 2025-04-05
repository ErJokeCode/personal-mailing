using dotenv.net;
using Microsoft.AspNetCore.Builder;

namespace Notify;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.InitialzieServices();
        app.MapRoutes();

        app.Run();
    }
}
