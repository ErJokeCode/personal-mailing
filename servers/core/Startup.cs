using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Core.Abstractions.MailService;
using Core.Abstractions.Parser;
using Core.Abstractions.UserAccesor;
using Core.Data;
using Core.Identity;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes;
using Core.Routes.Admins.Commands;
using Core.Routes.Events.Commands;
using Core.Signal;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using Scalar.AspNetCore;
using Shared.Context.Admins.Messages;
using Shared.Context.Students.Messages;
using Shared.Infrastructure.Handlers;
using Shared.Messages.Groups;
using Shared.Services.FileStorage;

namespace Core;

// TODO refactor startup class into a new folder with separation
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

        builder.Services.AddCors(
            (o) => o.AddDefaultPolicy(
                p => p.WithOrigins("http://localhost:4000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

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

        builder.Services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, SecretTokenAuthenticationHandler>("SecretTokenScheme", null);

        builder.Services.AddAuthorization(o =>
        {
            o.AddPolicy(SecretTokenAuthentication.Policy, (p) =>
            {
                p.RequireClaim(SecretTokenAuthentication.Claim);
            });

            o.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("SecretTokenScheme", IdentityConstants.ApplicationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });

        builder.Services.AddIdentity<Admin, IdentityRole<Guid>>((o) =>
        {
            o.Password.RequireDigit = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequiredUniqueChars = 0;
            o.Password.RequiredLength = 1;
            // FIXME doesnt allow "admin" as email so comment out for now
            // options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        builder.Services.ConfigureApplicationCookie(o =>
        {
            o.SlidingExpiration = true;
            o.SessionStore = new AdminTicketStore(builder.Services);

            o.Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },

                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddReverseProxy().LoadFromMemory(YarpReverseProxy.GetRoutes(), YarpReverseProxy.GetClusters());

        builder.Services.AddScoped<IUserAccessor, UserAccessor>();

        builder.Services.Configure<ParserOptions>(o =>
        {
            o.ParserUrl = Environment.GetEnvironmentVariable("PARSER_URL")!;
        });
        builder.Services.AddScoped<IParser, Parser>();

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

        builder.Services.Configure<MailServiceOptions>(o =>
        {
            o.MailServiceUrl = Environment.GetEnvironmentVariable("BOT_URL")!;
        });
        builder.Services.AddScoped<IMailService, TelegramBot>();

        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddProducer<AdminCreatedMessage>(AdminCreatedMessage.TopicName);
                rider.AddProducer<StudentAuthedMessage>(StudentAuthedMessage.TopicName);
                rider.AddProducer<StudentsUpdatedMessage>(StudentsUpdatedMessage.TopicName);
                rider.AddProducer<GroupsAddedMessage>(GroupsAddedMessage.TopicName);

                rider.UsingKafka((context, k) =>
                {
                    k.Host(Environment.GetEnvironmentVariable("KAFKA_URL"));
                });
            });
        });
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        var busControl = app.Services.GetRequiredService<IBusControl>();
        await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

        app.UseRequestLocalization((o) => o.SetDefaultCulture("ru"));

        if (app.Environment.IsDevelopment())
        {
            app.MigrateDatabase();
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Servers =
                [
                    new ScalarServer("http://localhost:5000")
                ];
                o.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.Fetch);
            });
        }

        await app.EnsureAdminCreated();
        await app.UpdateStudentsInfo();

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();
        app.UseHealthChecks("/healthy");
        app.MapHub<SignalHub>("/core-hub");

        app.MapReverseProxy(pipeline =>
        {
            pipeline.Use(async (context, next) =>
            {
                using var scope = app.Services.CreateScope();
                var userAccesor = scope.ServiceProvider.GetRequiredService<IUserAccessor>();
                var admin = await userAccesor.GetUserAsync();

                if (admin is not null)
                {
                    context.Request.Headers["x-user-info"] = JsonSerializer.Serialize(admin);
                }

                await next(context);
            });
        })
        .RequireAuthorization();
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

    private static async Task EnsureAdminCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateAdminCommand()
        {
            Email = Environment.GetEnvironmentVariable("MAIN_ADMIN_EMAIL")!,
            Password = Environment.GetEnvironmentVariable("MAIN_ADMIN_PASSWORD")!
        };

        await mediator.Send(command);
    }

    private static async Task UpdateStudentsInfo(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new UploadEventCommand();

        await mediator.Send(command);
    }
}