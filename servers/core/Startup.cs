using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using System.Security.Claims;
using Microsoft.AspNetCore.HttpLogging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using System;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Core.Utility;
using Core.Messages;
using Core.Handlers;
using Core.Identity;
using Core.Workers;

namespace Core;

public static class Startup
{
    public static void LoadEnv(string filePath)
    {
        var root = Directory.GetCurrentDirectory();
        filePath = Path.Combine(root, filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The file '{filePath}' does not exist.");

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static void CreateFolder(string name)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), name);
        Directory.CreateDirectory(path);
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        Action<HttpLoggingOptions> httpLoggingOptions = options =>
        {
            options.LoggingFields = HttpLoggingFields.All;
            options.RequestBodyLogLimit = 4096;
            options.ResponseBodyLogLimit = 4096;
        };
        builder.Services.AddHttpLogging(httpLoggingOptions);

        Action<CorsOptions> corsOptions = options =>
        {
            Action<CorsPolicyBuilder> corsPolicy = policy =>
            {
                policy.WithOrigins(new string[] { "http://localhost:5010", "http://localhost:5015" })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            };
            options.AddDefaultPolicy(corsPolicy);
        };
        builder.Services.AddCors(corsOptions);

        Action<IBusRegistrationConfigurator> busOptions = options =>
        {
            options.AddConsumer<ChatConsumer.StudentSentMessageConsumer>();

            Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator> busContext = (context, cfg) =>
            {
                Action<IRabbitMqHostConfigurator> rabbitOptions = rabbit =>
                {
                    rabbit.Username("guest");
                    rabbit.Password("guest");
                };

                cfg.Host("rabbitmq", "/", rabbitOptions);
                cfg.ConfigureEndpoints(context);
            };
            options.UsingRabbitMq(busContext);
        };
        builder.Services.AddMassTransit(busOptions);

        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        builder.Services.AddSignalR();

        var connection = builder.Configuration.GetConnectionString("Database");
        builder.Services.AddDbContext<CoreDb>(options => options.UseNpgsql(connection));

        builder.Services.AddPermissions();
        builder.Services.AddHostedService<ScheduleWorker>();

        Action<IdentityOptions> identityOptions = options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequiredLength = 1;
            // TODO in production probably add this back
            // doesnt allow "admin" as email so comment out for now
            // options.User.RequireUniqueEmail = true;
        };
        builder.Services.AddIdentityApiEndpoints<AdminUser>(identityOptions).AddEntityFrameworkStores<CoreDb>();
    }

    public static async Task InitialzieServices(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            MigrateDatabase(services);
            await CreateAdminUser(services);
        }

        app.UseHttpLogging();
        app.UseCors();
        app.MapCustomIdentityApi<AdminUser>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapReverseProxy();
        app.MapHub<SignalHub>("/signal");
    }

    public static void MigrateDatabase(IServiceProvider services)
    {
        var context = services.GetRequiredService<CoreDb>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }

    public static async Task CreateAdminUser(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<AdminUser>>();
        var userStore = services.GetRequiredService<IUserStore<AdminUser>>();
        var db = services.GetRequiredService<CoreDb>();
        await AdminHandler.CreateAdmin("admin", "admin", Permissions.All.Select(p => p.Claim).ToList(), userManager,
                                       userStore, db);
    }
}
