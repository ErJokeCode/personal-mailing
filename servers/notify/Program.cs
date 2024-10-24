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
        builder.ConfigureServices();

        var app = builder.Build();
        app.InitialzieServices();

        Handlers.Hub = app.Services.GetService<IHubContext<SignalHub>>();

        app.Run();
    }
}
