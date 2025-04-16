using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Features.Admins.Commands;
using Core.Identity;
using Core.Models;
using Core.Services.UserAccessor;
using Core.Setup;
using Core.Signal;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using Shared.Context.Admins.Messages;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Handlers;

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

        builder.Services.AddMappers(typeof(Program).Assembly);

        builder.Services.AddScoped<AdminService>();

        builder.Services.AddScoped<IUserAccessor, UserAccessor>();

        builder.Services.AddMassTransit(x =>
        {
            x.UsingInMemory();

            x.AddRider(rider =>
            {
                rider.AddProducer<AdminCreatedMessage>(AdminCreatedMessage.TopicName);

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
        var adminService = scope.ServiceProvider.GetRequiredService<AdminService>();

        var request = new CreateAdminCommand.Request()
        {
            Email = Environment.GetEnvironmentVariable("MAIN_ADMIN_EMAIL")!,
            Password = Environment.GetEnvironmentVariable("MAIN_ADMIN_PASSWORD")!
        };

        await adminService.CreateAdminAsync(request);
    }
}