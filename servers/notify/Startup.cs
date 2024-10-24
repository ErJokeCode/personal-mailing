using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Notify.Models;
using MassTransit;
using System;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Notify;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        Action<CorsOptions> corsOptions = options =>
        {
            Action<CorsPolicyBuilder> configurePolicy = policy =>
            { policy.WithOrigins(new string[] { "http://localhost:5000" }).AllowAnyHeader().AllowAnyMethod(); };
            options.AddDefaultPolicy(configurePolicy);
        };
        builder.Services.AddCors(corsOptions);

        Action<IBusRegistrationConfigurator> busOptions = options =>
        {
            options.AddConsumer<Handlers.NewStudentAuthedConsumer>();

            Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> configureBus = (context, cfg) =>
            {
                Action<IRabbitMqHostConfigurator> rabbitOptions = rabbit =>
                {
                    rabbit.Username("guest");
                    rabbit.Password("guest");
                };
                cfg.Host("rabbitmq", "/", rabbitOptions);

                cfg.ConfigureEndpoints(context);
            };
            options.UsingRabbitMq(configureBus);
        };
        builder.Services.AddMassTransit(busOptions);

        builder.Services.AddSignalR();
    }

    public static void InitialzieServices(this WebApplication app)
    {
        app.UseCors();
        app.MapHub<SignalHub>("/signal");
    }
}
