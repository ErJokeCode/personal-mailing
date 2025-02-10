using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data;
using Core.External.Parser;
using Core.External.TelegramBot;
using Core.Infrastructure.Handlers;
using Core.Infrastructure.Services;
using Core.Models;
using Core.Routes;
using Core.Routes.Admins.Commands;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;

namespace Core;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ExceptionHandler>();
        builder.Services.AddHttpClient();

        builder.Services.AddOpenApi();

        builder.Services.AddCors(
            (o) => o.AddDefaultPolicy(
                p => p.WithOrigins("http://localhost:5015")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connection));

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();

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

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.Configure<ParserOptions>(o =>
        {
            o.ParserUrl = builder.Configuration.GetConnectionString("Parser")!;
        });
        builder.Services.AddScoped<IParser, Parser>();

        builder.Services.Configure<TelegarmBotOptions>(o =>
        {
            o.TelegarmBotUrl = builder.Configuration.GetConnectionString("TelegramBot")!;
        });
        builder.Services.AddScoped<ITelegramBot, TelegramBot>();

        builder.Services.AddScoped<IUserAccessor, UserAccessor>();
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
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

        app.UseRequestLocalization((o) => o.SetDefaultCulture("ru"));
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler();

        app.MapReverseProxy();
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

    private static async Task EnsureAdminCreated(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var command = new CreateAdminCommand()
        {
            Email = app.Configuration["MainAdmin:Name"]!,
            Password = app.Configuration["MainAdmin:Password"]!
        };

        await mediator.Send(command);
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