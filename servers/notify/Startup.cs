using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Notify.Data;
using Notify.Features;
using Notify.Setup;
using Riok.Mapperly.Abstractions;
using Scalar.AspNetCore;
using Shared.Infrastructure.Handlers;

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
        builder.Services.AddOpenApi();

        builder.Services.AddMappers();
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        builder.Services.SetupDatabase();
        builder.Services.RegisterServices();

        builder.Services.SetupKafka();
    }

    public static void InitializeServices(this WebApplication app)
    {
        app.UseRequestLocalization((o) => o.SetDefaultCulture("ru"));
        app.MigrateDatabase();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Servers =
                [
                    new ScalarServer("http://localhost:5030")
                ];
                o.DefaultHttpClient = new(ScalarTarget.JavaScript, ScalarClient.Fetch);
            });
        }

        app.UseExceptionHandler();
        app.UseHealthChecks("/healthy");
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

    public static void AddMappers(this IServiceCollection services)
    {
        IEnumerable<Type> mappers = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<MapperAttribute>() != null && !t.IsAbstract && !t.IsInterface);

        foreach (var mapper in mappers)
        {
            services.AddTransient(mapper);
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