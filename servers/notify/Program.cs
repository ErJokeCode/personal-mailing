using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Notify.Models;
using MassTransit;

namespace Notify;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.ConfigureServices();

        var app = builder.Build();
        await app.InitialzieServices();

        Handlers.Hub = app.Services.GetService<IHubContext<SignalHub>>();

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

        builder.Services.AddSignalR();

        builder.Services.AddMassTransit(o =>
                                        {
                                            o.AddConsumer<Handlers.NewStudentAuthedConsumer>();

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
        app.MapHub<SignalHub>("/signal");
    }
}
