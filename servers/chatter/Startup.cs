using System;
using System.Linq;
using Chatter.Signal;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Chatter.Data;
using Scalar.AspNetCore;
using Shared.Infrastructure.Handlers;
using Shared.Services.FileStorage;
using Shared.Context.Admins.Messages;
using Shared.Context.Students.Messages;
using Chatter.Services.MailService;
using Chatter.Services.UserAccessor;
using Chatter.Features.Admins.Consumers;
using Shared.Infrastructure.Extensions;
using Chatter.Services;

namespace Chatter;

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

        builder.Services.AddMappers(typeof(Program).Assembly);

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

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.AddScoped<ChatService>();

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

                const string ServiceName = "chatter-";

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

        app.MapHub<SignalHub>("/chat-hub");
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