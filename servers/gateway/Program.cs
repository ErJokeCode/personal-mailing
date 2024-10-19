using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        app.Run();
    }

    public static async Task ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpLogging(o =>
                                        {
                                            o.LoggingFields = HttpLoggingFields.All;
                                            o.RequestBodyLogLimit = 4096;
                                            o.ResponseBodyLogLimit = 4096;
                                        });

        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    { policy.WithOrigins("http://client:5010").AllowAnyMethod().AllowAnyHeader(); });
            });
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        app.UseHttpLogging();
        app.UseCors();
        app.MapReverseProxy();
    }
}
