using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Notify.Models;
using NServiceBus;
using Shared.Messages;

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

    public static async Task ConfigureRabbitMQ()
    {
        var endpointConfiguration = new EndpointConfiguration("Notify");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.ConnectionString("host=rabbitmq");

        var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
    }

    public static async Task ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(new string[] { "http://localhost:5010" }).AllowAnyHeader().AllowAnyMethod();
                    });
            });
        builder.Services.AddSignalR();

        await ConfigureRabbitMQ();
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        app.UseCors();
        app.MapHub<SignalHub>("/signal");
    }
}
