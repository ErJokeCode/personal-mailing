using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Models;
using Shared.Messages;
using MassTransit;

namespace Core;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        app.MapPost("/auth", Handlers.HandleAuth);
        app.MapGet("/{id}/courses", Handlers.HandleCourses);

        app.Run();
    }

    public static async Task ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(new string[] { "http://localhost:5000" }).AllowAnyHeader().AllowAnyMethod();
                    });
            });

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));

        builder.Services.AddMassTransit(o =>
                                        {
                                            o.UsingRabbitMq((context, cfg) =>
                                                            {
                                                                cfg.Host("rabbitmq", "/",
                                                                         h =>
                                                                         {
                                                                             h.Username("guest");
                                                                             h.Password("guest");
                                                                         });

                                                                cfg.ConfigureEndpoints(context);
                                                            });
                                        });
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        app.UseCors();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<CoreDb>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }
    }
}
