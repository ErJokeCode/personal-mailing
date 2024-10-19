using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using Shared.Messages;

namespace Notify;

public static class Program
{
    static IHubContext<SignalHub> _hub = null;

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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

        var app = builder.Build();

        var endpointConfiguration = new EndpointConfiguration("Notify");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();

        var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
        transport.UseConventionalRoutingTopology(QueueType.Quorum);
        transport.ConnectionString("host=rabbitmq");

        var endpointInstance = await NServiceBus.Endpoint.Start(endpointConfiguration);

        app.UseCors();
        app.MapHub<SignalHub>("/signal");

        _hub = app.Services.GetService<IHubContext<SignalHub>>();

        app.Run();

        await endpointInstance.Stop();
    }

    public class UpdateStudentsHandler : IHandleMessages<NewStudentAuth>
    {
        public async Task Handle(NewStudentAuth message, IMessageHandlerContext context)
        {
            await _hub.Clients.All.SendAsync("NewStudentAuth", message.Student);
        }
    }

    public class SignalHub : Hub
    {
    }
}
