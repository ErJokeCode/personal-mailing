using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Models;
using Shared.Messages;
using NServiceBus;

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

    private static async Task<IEndpointInstance> ConfigureRabbitMQ()
    {
        var endpointConfiguration = new EndpointConfiguration("Core");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.ConnectionString("host=rabbitmq");

        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(NewStudentAuth), "Notify");

        var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);
        return endpointInstance;
    }

    public static async Task ConfigureServices(this WebApplicationBuilder builder)
    {
        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));

        var endpointInstance = await ConfigureRabbitMQ();
        builder.Services.AddSingleton<IEndpointInstance>(endpointInstance);
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
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
