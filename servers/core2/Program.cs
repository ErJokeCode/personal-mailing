using Core;
using Microsoft.AspNetCore.Builder;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureServices();

        var app = builder.Build();
        app.InitialzieServices();
        app.RegisterRouteDefinitions();

        app.Run();
    }
}
