using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        var app = builder.Build();

        app.UseHttpLogging();
        app.UseCors();
        app.MapReverseProxy();

        app.Run();
    }
}
