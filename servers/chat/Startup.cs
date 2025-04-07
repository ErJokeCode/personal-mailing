using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Signal;
using dotenv.net;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Notify.Abstractions.MailService;
using Notify.Abstractions.UserAccessor;
using Notify.Consumers.Admins;
using Notify.Consumers.Groups;
using Notify.Consumers.Students;
using Notify.Data;
using Notify.Infrastructure.Services;
using Notify.Routes;
using Scalar.AspNetCore;
using Shared.Abstractions.FileStorage;
using Shared.Infrastructure.Handlers;
using Shared.Infrastructure.Services;
using Shared.Messages.Admins;
using Shared.Messages.Groups;
using Shared.Messages.Students;

namespace Notify;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddHttpClient();
        builder.Services.AddHealthChecks();
        builder.Services.AddSignalR();

        builder.Services.AddOpenApi();

        var host = Environment.GetEnvironmentVariable("POSTGRES_URL");
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");

        var connection = $"Host={host};Port={5432};Database={database};Username={user};Password={password};Include Error Detail=True";
        builder.Services.AddDbContext<AppDbContext>(o =>
        {
            o.UseNpgsql(connection);
            o.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
        });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddScoped<IUserAccessor, UserAccessor>();

        builder.Services.Configure<MailServiceOptions>(o =>
        {
            o.MailServiceUrl = Environment.GetEnvironmentVariable("BOT_URL")!;
        });
        builder.Services.AddScoped<IMailService, TelegramBot>();

        builder.Services.AddMinio(
            configureClient => configureClient
                .WithEndpoint(Environment.GetEnvironmentVariable("MINIO_URL"))
                .WithCredentials(Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY"), Environment.GetEnvironmentVariable("MINIO_SECRET_KEY"))
                .WithSSL(false)
                .Build()
        );

        builder.Services.Configure<FileStorageOptions>(o =>
        {
            o.BucketName = Environment.GetEnvironmentVariable("MINIO_BUCKET")!;
        });
        builder.Services.AddSingleton<IFileStorage, MinioStorage>();

        builder.Services.AddSingleton<IUserIdProvider, HttpContextUserIdProvider>();

        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddConsumer<AdminCreatedConsumer>();
                rider.AddConsumer<StudentAuthedConsumer>();
                rider.AddConsumer<StudentsUpdatedConsumer>();
                rider.AddConsumer<GroupsAddedConsumer>();

                const string ServiceName = "chat-";

                rider.UsingKafka((context, k) =>
                {
                    k.Host(Environment.GetEnvironmentVariable("KAFKA_URL"));

                    k.TopicEndpoint<AdminCreatedMessage>(AdminCreatedMessage.TopicName, ServiceName + AdminCreatedMessage.TopicName, e =>
                    {
                        e.CreateIfMissing();
                        e.ConfigureConsumer<AdminCreatedConsumer>(context);
                    });

                    k.TopicEndpoint<StudentAuthedMessage>(StudentAuthedMessage.TopicName, ServiceName + StudentAuthedMessage.TopicName, e =>
                    {
                        e.CreateIfMissing();
                        e.ConfigureConsumer<StudentAuthedConsumer>(context);
                    });

                    k.TopicEndpoint<StudentsUpdatedMessage>(StudentsUpdatedMessage.TopicName, ServiceName + StudentsUpdatedMessage.TopicName, e =>
                    {
                        e.CreateIfMissing();
                        e.ConfigureConsumer<StudentsUpdatedConsumer>(context);
                    });

                    k.TopicEndpoint<GroupsAddedMessage>(GroupsAddedMessage.TopicName, ServiceName + GroupsAddedMessage.TopicName, e =>
                    {
                        e.CreateIfMissing();
                        e.ConfigureConsumer<GroupsAddedConsumer>(context);
                    });
                });
            });
        });
    }

    public static void InitialzieServices(this WebApplication app)
    {
        app.UseRequestLocalization((o) => o.SetDefaultCulture("ru"));

        if (app.Environment.IsDevelopment())
        {
            app.MigrateDatabase();
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Servers =
                [
                    new ScalarServer("http://localhost:5040")
                ];
                o.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.Fetch);
            });
        }

        app.UseExceptionHandler();
        app.UseHealthChecks("/healthy");

        app.MapHub<SignalHub>("/hub");
    }

    public static void MapRoutes(this WebApplication app)
    {
        IEnumerable<IRoute> routes = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IRoute)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IRoute>();

        foreach (var route in routes)
        {
            route.MapRoutes(app);
        }
    }

    private static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}