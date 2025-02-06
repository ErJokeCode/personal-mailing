using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core.Data;
using Core.Infrastructure.Behaviors;
using Core.Infrastructure.Middleware;
using Core.Models;
using Core.Routes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class Startup
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
            (o) => o.AddDefaultPolicy(
                p => p.WithOrigins("http://localhost:5015")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
            )
        );

        // var connection = builder.Configuration.GetConnectionString("Database");
        // builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));
        // FIXME use sqlite database for testing
        builder.Services.AddDbContext<CoreDb>((o) => o.UseSqlite("Data Source=database.db"));

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
        .AddEntityFrameworkStores<CoreDb>()
        .AddDefaultTokenProviders();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    public static void RegisterRouteDefinitions(this WebApplication app)
    {
        IEnumerable<IRouteDefinition> routeDefinitions = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IRouteDefinition)) && !t.IsAbstract && !t.IsInterface)
            .Select(Activator.CreateInstance)
            .Cast<IRouteDefinition>();

        foreach (var routeDefinition in routeDefinitions)
        {
            routeDefinition.RegisterRoutes(app);
        }
    }

    public static void InitialzieServices(this WebApplication app)
    {
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRequestLocalization((o) => o.SetDefaultCulture("ru"));
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}