using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Data;
using Core.Features.Admins.Commands;
using Core.Services.UserAccessor;
using Core.Setup;
using Core.Signal;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
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

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddReverseProxy().LoadFromMemory(ProxySetup.GetRoutes(), ProxySetup.GetClusters());
        builder.Services.AddMappers(typeof(Program).Assembly);

        builder.Services.RegisterServices();

        builder.Services.SetupDatabase();
        builder.Services.SetupIdentity();
        builder.Services.SetupKafka();
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